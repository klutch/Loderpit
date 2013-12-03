using System;
using System.Collections.Generic;
using System.Diagnostics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using Microsoft.Xna.Framework;
using SFML.Window;
using Loderpit.Formations;
using Loderpit.Managers;
using Loderpit.Components;
using Loderpit.Skills;

namespace Loderpit.Systems
{
    using Key = Keyboard.Key;

    public class TeamSystem : ISystem
    {
        private int _selectedTeammate;
        private GroupComponent _playerGroup;

        // General skill variables
        private Skill _initializingSkill;

        // CreateRope skill variables
        private Vector2 _createRopeAnchor;

        // CreateBridge skill variables
        private Vector2 _createBridgeAnchorA;
        private Vector2 _createBridgeAnchorB;
        private bool _firstBridgeAnchorSet = false;

        public SystemType systemType { get { return SystemType.Team; } }
        public int selectedTeammate { get { return _selectedTeammate; } }
        public Vector2 createRopeAnchor { get { return _createRopeAnchor; } }
        public Vector2 createBridgeAnchorA { get { return _createBridgeAnchorA; } }
        public Vector2 createBridgeAnchorB { get { return _createBridgeAnchorB; } }
        public GroupComponent playerGroup { get { return _playerGroup; } set { _playerGroup = value; } }
        public Skill initializingSkill { get { return _initializingSkill; } }

        public TeamSystem()
        {
        }

        // Select next teammate
        public void selectNextTeammate()
        {
            _selectedTeammate = _selectedTeammate + 1 > _playerGroup.entities.Count - 1 ? 0 : _selectedTeammate + 1;
        }

        // Select previous teammate
        public void selectPreviousTeammate()
        {
            _selectedTeammate = _selectedTeammate - 1 < 0 ? _playerGroup.entities.Count - 1 : _selectedTeammate - 1;
        }

        // Get entities in the player's team
        public List<int> getTeamEntities()
        {
            return _playerGroup.entities;
        }

        // Get a teammate's entity id
        public int getTeammateEntityId(int formationSlot)
        {
            return _playerGroup.entities[formationSlot];
        }

        // Handles input for the player's team
        private void handleInput()
        {
            if (Game.inFocus)
            {
                int selectedEntityId = getTeammateEntityId(_selectedTeammate);
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(selectedEntityId);
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(selectedEntityId);
                List<Skill> skills = skillsComponent.activatableSkills;
                int minNum = (int)Key.Num1;
                int maxNum = (int)Key.Num8;
                int count = 0;

                // Read key states
                if (Game.newKeyState.isPressed(Key.A) && Game.oldKeyState.isReleased(Key.A))
                {
                    selectPreviousTeammate();
                }
                if (Game.newKeyState.isPressed(Key.D) && Game.oldKeyState.isReleased(Key.D))
                {
                    selectNextTeammate();
                }
                if (Game.newKeyState.isPressed(Key.Q) && Game.oldKeyState.isReleased(Key.Q))
                {
                    SystemManager.groupSystem.decreaseActiveFormationSpeed(_playerGroup.entityId);
                }
                if (Game.newKeyState.isPressed(Key.E) && Game.oldKeyState.isReleased(Key.E))
                {
                    SystemManager.groupSystem.increaseActiveFormationSpeed(_playerGroup.entityId);
                }
                if (Game.newKeyState.isPressed(Key.Escape))
                {
                    _initializingSkill = null;
                }
                for (int i = minNum; i < maxNum; i++)
                {
                    if (count == skills.Count)
                    {
                        break;
                    }

                    if (Game.newKeyState.isPressed((Key)i) && Game.oldKeyState.isReleased((Key)i))
                    {
                        Skill skill = skills[count];

                        // Make sure skill is ready to be used, with the exception of melee/ranged attacks
                        if (skill.type == SkillType.MeleeAttack || skill.type == SkillType.MeleeAttack)
                        {
                            _initializingSkill = skill;
                        }
                        else if (skill.cooldown == 0)
                        {
                            _initializingSkill = skill;
                        }
                    }

                    count++;
                }

                // Handle actions
                if (_initializingSkill != null)
                {
                    if (_initializingSkill.cooldown == 0)
                    {
                        switch (_initializingSkill.type)
                        {
                            // Common
                            case SkillType.MeleeAttack:
                            case SkillType.RangedAttack:
                                handleInitializeAttack(selectedEntityId);
                                break;

                            // Engineer
                            case SkillType.ThrowRope:
                                handleInitializeThrowRope(selectedEntityId);
                                break;
                            case SkillType.BuildBridge:
                                handleInitializeBuildBridge(selectedEntityId);
                                break;

                            // Archer
                            case SkillType.PowerShot:
                                handleInitializePowerShot(selectedEntityId);
                                break;

                            // Fighter
                            case SkillType.PowerSwing:
                                handleInitializePowerSwing(selectedEntityId);
                                break;

                            // Mage
                            case SkillType.Fireball:
                                handleInitializeFireball(selectedEntityId);
                                break;
                        }
                    }
                }
            }
        }

        // Handle initialization of a melee attack
        private void handleInitializeAttack(int selectedEntityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                if (_initializingSkill.type == SkillType.MeleeAttack)
                {
                    SystemManager.skillSystem.performMeleeAttackSkill(selectedEntityId, _initializingSkill as MeleeAttackSkill, Game.worldMouse);
                }
                else if (_initializingSkill.type == SkillType.RangedAttack)
                {
                    SystemManager.skillSystem.performRangedAttackSkill(selectedEntityId, _initializingSkill as RangedAttackSkill, Game.worldMouse);
                }
                _initializingSkill = null;
            }
        }

        // Handle initialization of the throw rope skill
        private void handleInitializeThrowRope(int selectedEntityId)
        {
            _createRopeAnchor = Game.worldMouse;
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performThrowRopeSkill(selectedEntityId, _initializingSkill as ThrowRopeSkill, _createRopeAnchor);
                _initializingSkill = null;
            }
        }

        // Handle initialization of the build bridge skill
        private void handleInitializeBuildBridge(int selectedEntityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                // Set first anchor
                _createBridgeAnchorA = Game.worldMouse;
                _firstBridgeAnchorSet = true;
            }
            else if (!Game.newMouseState.isLeftButtonPressed && Game.oldMouseState.isLeftButtonPressed)
            {
                // Perform action
                SystemManager.skillSystem.performBuildBridgeSkill(selectedEntityId, _initializingSkill as BuildBridgeSkill, _createBridgeAnchorA, _createBridgeAnchorB);
                _firstBridgeAnchorSet = false;
                _initializingSkill = null;
            }
            if (_firstBridgeAnchorSet)
            {
                // Set second anchor
                _createBridgeAnchorB = Game.worldMouse;
            }
            else
            {
                // Reset anchors
                _createBridgeAnchorA = Game.worldMouse;
                _createBridgeAnchorB = Game.worldMouse;
            }
        }

        // Handle initialize power shot skill
        private void handleInitializePowerShot(int selectedEntityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performPowerShotSkill(selectedEntityId, _initializingSkill as PowerShotSkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize power swing skill
        private void handleInitializePowerSwing(int selectedEntityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performPowerSwingSkill(selectedEntityId, _initializingSkill as PowerSwingSkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize fireball skill
        private void handleInitializeFireball(int selectedEntityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performFireballSkill(selectedEntityId, _initializingSkill as FireballSkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        public void update()
        {
            List<int> entities = getTeamEntities();

            // Handle input
            handleInput();
        }
    }
}
