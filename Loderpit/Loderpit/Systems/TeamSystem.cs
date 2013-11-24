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
                        _initializingSkill = skills[count];
                    }

                    count++;
                }

                // Handle actions
                if (_initializingSkill != null)
                {
                    switch (_initializingSkill.type)
                    {
                        // Melee and ranged attacks
                        case SkillType.MeleeAttack:
                        case SkillType.RangedAttack:
                            handleInitializeAttack(selectedEntityId);
                            break;

                        // Throwing rope
                        case SkillType.ThrowRope:
                            handleInitializeThrowRope(selectedEntityId);
                            break;

                        // Building a bridge
                        case SkillType.BuildBridge:
                            handleInitializeBuildBridge(selectedEntityId);
                            break;
                    }
                }
            }
        }

        // Handle initialization of a melee attack
        private void handleInitializeAttack(int selectedEntityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                FactionComponent factionComponent = EntityManager.getFactionComponent(selectedEntityId);
                float attackRange = _initializingSkill.type == SkillType.MeleeAttack ? (_initializingSkill as MeleeAttackSkill).range : (_initializingSkill as RangedAttackSkill).range;
                List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(Game.worldMouse);

                foreach (Fixture fixture in fixtures)
                {
                    int entityId;
                    FactionComponent targetFactionComponent;

                    // Skip bodies without a user data
                    if (fixture.Body.UserData == null)
                    {
                        continue;
                    }

                    entityId = (int)fixture.Body.UserData;

                    // Skip entities without a faction component
                    if ((targetFactionComponent = EntityManager.getFactionComponent(entityId)) == null)
                    {
                        continue;
                    }

                    // Skip over non-attackable entities
                    if (!SystemManager.combatSystem.isFactionAttackable(factionComponent.faction, targetFactionComponent.faction))
                    {
                        continue;
                    }

                    SystemManager.combatSystem.startActiveAttack(selectedEntityId, entityId, attackRange);
                    break;
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
                ThrowRopeSkill throwRopeSkill = _initializingSkill as ThrowRopeSkill;
                Formation activeFormation = _playerGroup.activeFormation;
                CreateRopeComponent createRopeComponent = new CreateRopeComponent(selectedEntityId, _createRopeAnchor);
                LimitedRangeFormation formation = new LimitedRangeFormation(_playerGroup.entities, activeFormation.position, activeFormation.speed, float.MinValue, _createRopeAnchor.X - 4f);

                EntityManager.addComponent(selectedEntityId, createRopeComponent);
                EntityManager.addComponent(selectedEntityId, new PositionTargetComponent(selectedEntityId, _createRopeAnchor.X, throwRopeSkill.range));
                createRopeComponent.formationToRemove = formation;
                _playerGroup.addFormation(formation);
                _initializingSkill = null;
            }
        }

        // Handle initialization of the build bridge skill
        private void handleInitializeBuildBridge(int selectedEntityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                _createBridgeAnchorA = Game.worldMouse;
                _firstBridgeAnchorSet = true;
            }
            else if (!Game.newMouseState.isLeftButtonPressed && Game.oldMouseState.isLeftButtonPressed)
            {
                BuildBridgeSkill buildBridgeSkill = _initializingSkill as BuildBridgeSkill;
                Formation activeFormation = _playerGroup.activeFormation;
                PositionComponent positionComponent = EntityManager.getPositionComponent(selectedEntityId);
                float distanceA = Math.Abs(_createBridgeAnchorA.X - positionComponent.position.X);
                float distanceB = Math.Abs(_createBridgeAnchorB.X - positionComponent.position.X);
                Vector2 closestAnchor = distanceA > distanceB ? _createBridgeAnchorB : _createBridgeAnchorA;
                CreateBridgeComponent createBridgeComponent = new CreateBridgeComponent(selectedEntityId, _createBridgeAnchorA, _createBridgeAnchorB);
                LimitedRangeFormation formation = new LimitedRangeFormation(_playerGroup.entities, activeFormation.position, activeFormation.speed, float.MinValue, closestAnchor.X - 2f);

                _firstBridgeAnchorSet = false;
                EntityManager.addComponent(selectedEntityId, createBridgeComponent);
                EntityManager.addComponent(selectedEntityId, new PositionTargetComponent(selectedEntityId, closestAnchor.X, buildBridgeSkill.range));
                createBridgeComponent.formationToRemove = formation;
                _playerGroup.addFormation(formation);
                _initializingSkill = null;
            }
            if (_firstBridgeAnchorSet)
            {
                _createBridgeAnchorB = Game.worldMouse;
            }
            else
            {
                _createBridgeAnchorA = Game.worldMouse;
                _createBridgeAnchorB = Game.worldMouse;
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
