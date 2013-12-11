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
using Loderpit.Components.SpellEffects;
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

        // ThrowRope skill variables
        private Vector2 _createRopeAnchor;

        // BuildBridge skill variables
        private Vector2 _createBridgeAnchorA;
        private Vector2 _createBridgeAnchorB;
        private bool _firstBridgeAnchorSet = false;

        // ArrowTime skill variables
        private bool _initializingArrowTime = false;
        private List<int> _arrowTimeTargets;
        private int _slowMotionEntityId;

        public SystemType systemType { get { return SystemType.Team; } }
        public int selectedTeammate { get { return _selectedTeammate; } }
        public Vector2 createRopeAnchor { get { return _createRopeAnchor; } }
        public Vector2 createBridgeAnchorA { get { return _createBridgeAnchorA; } }
        public Vector2 createBridgeAnchorB { get { return _createBridgeAnchorB; } }
        public GroupComponent playerGroup { get { return _playerGroup; } set { _playerGroup = value; } }
        public Skill initializingSkill { get { return _initializingSkill; } }

        public TeamSystem()
        {
            _arrowTimeTargets = new List<int>();
        }

        // Select next teammate
        public void selectNextTeammate()
        {
            _selectedTeammate = _selectedTeammate + 1 > _playerGroup.entities.Count - 1 ? 0 : _selectedTeammate + 1;
            _initializingSkill = null;
        }

        // Select previous teammate
        public void selectPreviousTeammate()
        {
            _selectedTeammate = _selectedTeammate - 1 < 0 ? _playerGroup.entities.Count - 1 : _selectedTeammate - 1;
            _initializingSkill = null;
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

                            // Defender
                            case SkillType.ShieldBash:
                                handleInitializeShieldBash(selectedEntityId);
                                break;
                            case SkillType.Riposte:
                                handleInitializeRiposte(selectedEntityId);
                                break;
                            case SkillType.GolemStance:
                                handleInitializeGolemStance(selectedEntityId);
                                break;

                            // Engineer
                            case SkillType.ThrowRope:
                                handleInitializeThrowRope(selectedEntityId);
                                break;
                            case SkillType.BuildBridge:
                                handleInitializeBuildBridge(selectedEntityId);
                                break;
                            case SkillType.ProximityMine:
                                handleInitializeProximityMine(selectedEntityId);
                                break;

                            // Archer
                            case SkillType.PowerShot:
                                handleInitializePowerShot(selectedEntityId);
                                break;
                            case SkillType.ArrowTime:
                                handleInitializeArrowTime(selectedEntityId);
                                break;
                            case SkillType.Volley:
                                handleInitializeVolley(selectedEntityId);
                                break;

                            // Fighter
                            case SkillType.PowerSwing:
                                handleInitializePowerSwing(selectedEntityId);
                                break;
                            case SkillType.Fatality:
                                handleInitializeFatality(selectedEntityId);
                                break;
                            case SkillType.Frenzy:
                                handleInitializeFrenzy(selectedEntityId);
                                break;

                            // Mage
                            case SkillType.Fireball:
                                handleInitializeFireball(selectedEntityId);
                                break;
                            case SkillType.RainOfFire:
                                handleInitializeRainOfFire(selectedEntityId);
                                break;

                            // Healer
                            case SkillType.HealingBlast:
                                handleInitializeHealingBlast(selectedEntityId);
                                break;
                            case SkillType.Infusion:
                                handleInitializeInfusion(selectedEntityId);
                                break;
                            case SkillType.Dispel:
                                handleInitializeDispel(selectedEntityId);
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

        // Handle initialize shield bash skill
        private void handleInitializeShieldBash(int entityId)
        {
            SystemManager.skillSystem.performShieldBashSkill(entityId, _initializingSkill as ShieldBashSkill);
            _initializingSkill = null;
        }

        // Handle initialize healing blast skill
        private void handleInitializeHealingBlast(int entityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performHealingBlastSkill(entityId, _initializingSkill as HealingBlastSkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize proximity mine skill
        private void handleInitializeProximityMine(int entityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performProximityMineSkill(entityId, _initializingSkill as ProximityMineSkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize fatality skill
        private void handleInitializeFatality(int entityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performFatalitySkill(entityId, _initializingSkill as FatalitySkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize infusion skill
        private void handleInitializeInfusion(int entityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                int targetEntityId = Helpers.findEntityWithinRange(Game.worldMouse, 1f, factionComponent.faction);

                if (targetEntityId != -1)
                {
                    SystemManager.skillSystem.performInfusionSkill(entityId, _initializingSkill as InfusionSkill, targetEntityId);
                    _initializingSkill = null;
                }
            }
        }

        // Handle initialize rain of fire skill
        private void handleInitializeRainOfFire(int entityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performRainOfFireSkill(entityId, _initializingSkill as RainOfFireSkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize dispel skill
        private void handleInitializeDispel(int entityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                int targetEntityId = Helpers.findEntityWithinRange(Game.worldMouse, 1f, factionComponent.faction);

                if (targetEntityId != -1)
                {
                    SystemManager.skillSystem.performDispelSkill(entityId, _initializingSkill as DispelSkill, targetEntityId);
                    _initializingSkill = null;
                }
            }
        }

        // Handle initialize arrow time skill
        private void handleInitializeArrowTime(int entityId)
        {
            ArrowTimeSkill arrowTimeSkill = _initializingSkill as ArrowTimeSkill;

            // Handle setup
            if (!_initializingArrowTime)
            {
                _slowMotionEntityId = EntityFactory.createSlowMotionSpell(arrowTimeSkill.timeToLive);
                _initializingArrowTime = true;
            }

            // Check for end of setup
            if (!EntityManager.doesEntityExist(_slowMotionEntityId))
            {
                if (_arrowTimeTargets.Count > 0)
                {
                    SystemManager.skillSystem.performArrowTimeSkill(entityId, arrowTimeSkill, new List<int>(_arrowTimeTargets));
                }

                _slowMotionEntityId = -1;
                _initializingArrowTime = false;
                _initializingSkill = null;
                _arrowTimeTargets.Clear();
                return;
            }

            // Accumulate targets
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                int targetEntityId = Helpers.findEntityWithinRange(Game.worldMouse, 1f, new List<Faction>(new [] { Faction.Enemy, Faction.Neutral }), entityId);

                if (targetEntityId != -1 && !_arrowTimeTargets.Contains(targetEntityId))
                {
                    _arrowTimeTargets.Add(targetEntityId);
                }
            }
        }

        // Handle initialize volley skill
        private void handleInitializeVolley(int entityId)
        {
            if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                SystemManager.skillSystem.performVolleySkill(entityId, _initializingSkill as VolleySkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize riposte skill
        private void handleInitializeRiposte(int entityId)
        {
            SystemManager.skillSystem.performRiposteSkill(entityId, _initializingSkill as RiposteSkill);
            _initializingSkill = null;
        }

        // Handle initialize golem stance
        private void handleInitializeGolemStance(int entityId)
        {
            bool isSkillAlreadyActive = false;
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(entityId);

            // Check if golem stance is already active
            foreach (int spellId in affectedBySpellEntitiesComponent.spellEntities)
            {
                SpellTypeComponent spellTypeComponent = EntityManager.getSpellTypeComponent(spellId);

                if (spellTypeComponent != null && spellTypeComponent.spellType == SpellType.GolemStance)
                {
                    isSkillAlreadyActive = true;
                    break;
                }
            }

            if (isSkillAlreadyActive)
            {
                // Disable skill
                SystemManager.skillSystem.disableGolemStance(entityId);
                _initializingSkill = null;
            }
            else if (Game.newMouseState.isLeftButtonPressed && !Game.oldMouseState.isLeftButtonPressed)
            {
                // Enable skill
                SystemManager.skillSystem.enableGolemStance(entityId, _initializingSkill as GolemStanceSkill, Game.worldMouse);
                _initializingSkill = null;
            }
        }

        // Handle initialize frenzy skill
        private void handleInitializeFrenzy(int entityId)
        {
            SystemManager.skillSystem.performFrenzySkill(entityId, _initializingSkill as FrenzySkill);
            _initializingSkill = null;
        }

        public void update()
        {
            List<int> entities = getTeamEntities();

            // Handle input
            handleInput();
        }
    }
}
