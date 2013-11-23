using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;
using Loderpit.Formations;

namespace Loderpit.Systems
{
    /*
     * Types of Attacks:
     *   Passive -- Stay in formation, attacking when possible
     *   Active -- Break formation to move to and attack a target
     *   
     * Components used for passive attacks:
     *   None -- Attackable targets are found automatically and attacked if in attack range.
     *   
     * Components used for active attacks:
     *   PositionTarget -- Break formation to move to a target
     *   CombatTarget -- Focus on a specific entity rather than whatever is in range
     */
    public class CombatSystem : ISystem
    {
        private const float SKILL_RANGE_TOLERANCE = 0.2f;
        private Random _rng;

        public SystemType systemType { get { return SystemType.Combat; } }

        public CombatSystem()
        {
            _rng = new Random();
        }

        // Check to see if a faction is attackable by the other (this may get more complex later)
        public bool isFactionAttackable(Faction attackerFaction, Faction defenderFaction)
        {
            return attackerFaction != defenderFaction;
        }

        // A temporary function to add text to the screen
        private void addMessage(int entityId, string value)
        {
            PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);

            ScreenManager.levelScreen.addTemporaryWorldText(value, positionComponent.position);
        }

        // Start an active attack
        public void startActiveAttack(int attackerId, int defenderId, float attackRange)
        {
            CharacterComponent defenderCharacter;
            DestructibleObstacleComponent destructibleObstacleComponent;
            Body body = null;

            if ((defenderCharacter = EntityManager.getCharacterComponent(defenderId)) != null)
            {
                body = defenderCharacter.body;
            }
            else if ((destructibleObstacleComponent = EntityManager.getDestructibleObstacleComponent(defenderId)) != null)
            {
                body = destructibleObstacleComponent.body;
            }
            else
            {
                return;
            }

            EntityManager.addComponent(attackerId, new PositionTargetComponent(attackerId, body, attackRange));
            EntityManager.addComponent(attackerId, new CombatTargetComponent(attackerId, defenderId));
        }

        // End an active attack
        public void endActiveAttack(int attackerId)
        {
            EntityManager.removeComponent(attackerId, ComponentType.PositionTarget);
            EntityManager.removeComponent(attackerId, ComponentType.CombatTarget);
        }

        /* Attack an entity
         *   The attacking entity makes an attack roll.
         *   If the attack roll equals or beats the defenders armor class, the attacker makes a hit roll.
         *   
         * Attack Roll:
         *  d20 + strength modifier
         *  
         * Hit Roll:
         *  Weapon damage + strength modifier
         */
        public void attack(int attackerId, int defenderId)
        {
            StatsComponent attackerStats = EntityManager.getStatsComponent(attackerId);
            StatsComponent defenderStats = EntityManager.getStatsComponent(defenderId);
            int attackRoll = Roller.roll("d20") + SystemManager.statSystem.getStatModifier(attackerStats.strength);
            int defenderArmorClass = SystemManager.statSystem.getArmorClass(defenderId);

            if (attackRoll >= defenderArmorClass)
            {
                // Hit
                int hitRoll = Roller.roll("1d10");

                defenderStats.currentHp -= hitRoll;
                addMessage(defenderId, "-" + hitRoll.ToString());

                // Check for zero health
                if (defenderStats.currentHp == 0)
                {
                    handleZeroHealth(attackerId, defenderId);
                }
            }
            else
            {
                // Miss
                addMessage(attackerId, "Miss");
            }
        }

        // Handle an entity with zero health
        private void handleZeroHealth(int attackerId, int defenderId)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(defenderId);
            CombatTargetComponent combatTargetComponent = EntityManager.getCombatTargetComponent(attackerId);
            List<int> allCombatEntities;

            if (factionComponent.faction == Faction.Player)
            {
                // Incapacitate the entity
                EntityManager.addComponent(defenderId, new IncapacitatedComponent(defenderId));
            }
            else
            {
                // Kill the entity
                killEntity(defenderId);
            }

            // Find any entities engaged in active combat with the defender, and end their attacks
            allCombatEntities = EntityManager.getEntitiesPossessing(ComponentType.CombatTarget);
            foreach (int entityId in allCombatEntities)
            {
                CombatTargetComponent entityCombatTarget = EntityManager.getCombatTargetComponent(entityId);

                if (entityCombatTarget.targetId == defenderId)
                {
                    endActiveAttack(entityId);
                }
            }
        }

        // Handles all the logic required to remove a killed entity from the game
        public void killEntity(int entityId)
        {
            GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);
            CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
            DestructibleObstacleComponent destructibleObstacleComponent = EntityManager.getDestructibleObstacleComponent(entityId);

            // Remove from group
            if (groupComponent != null)
            {
                groupComponent.entities.Remove(entityId);
            }

            // Handle character removal
            if (characterComponent != null)
            {
                SystemManager.physicsSystem.world.RemoveBody(characterComponent.body);
                SystemManager.physicsSystem.world.RemoveBody(characterComponent.feet);
            }

            // Handle destructible obstacle removal
            if (destructibleObstacleComponent != null)
            {
                SystemManager.physicsSystem.world.RemoveBody(destructibleObstacleComponent.body);

                foreach (KeyValuePair<int, SplitFormation> entityFormationPair in destructibleObstacleComponent.formationsToRemove)
                {
                    GroupComponent formationGroup = EntityManager.getGroupComponent(entityFormationPair.Key);

                    formationGroup.removeFormation(entityFormationPair.Value);
                }
            }

            EntityManager.destroyEntity(entityId);
        }

        // Handle skill cooldowns
        private void handleSkillCooldowns(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);

                foreach (Skill skill in skillsComponent.skills)
                {
                    skill.cooldown--;
                    if (skill.cooldown < 0)
                    {
                        // TODO: Use different delay values depending on skill type
                        skill.cooldown = statsComponent.attackDelay;
                    }
                }
            }
        }

        // Handle melee attacks
        private void handleMeleeAttacks(List<int> attackerEntities, List<int> attackableEntities)
        {
            foreach (int attackerId in attackerEntities)
            {
                if (EntityManager.doesEntityExist(attackerId))  // entity could have been killed earlier this frame
                {
                    SkillsComponent attackerSkills = EntityManager.getSkillsComponent(attackerId);
                    MeleeAttackSkill attackerMeleeAttackSkill = attackerSkills.getSkill(SkillType.MeleeAttack) as MeleeAttackSkill;

                    if (attackerMeleeAttackSkill != null)   // attacker has a melee skill
                    {
                        PositionComponent attackerPositionComponent = EntityManager.getPositionComponent(attackerId);
                        FactionComponent attackerFactionComponent = EntityManager.getFactionComponent(attackerId);

                        if (attackerMeleeAttackSkill.cooldown == 0)   // ready to attack
                        {
                            foreach (int defenderId in attackableEntities)
                            {
                                if (EntityManager.doesEntityExist(defenderId))  // entity could have been killed earlier this frame
                                {
                                    PositionComponent defenderPositionComponent = EntityManager.getPositionComponent(defenderId);
                                    FactionComponent defenderFactionComponent = EntityManager.getFactionComponent(defenderId);
                                    Vector2 relative = defenderPositionComponent.position - attackerPositionComponent.position;
                                    bool isDefenderWithinRange = relative.Length() <= (attackerMeleeAttackSkill.range + SKILL_RANGE_TOLERANCE);
                                    bool isDefenderAttackable = isFactionAttackable(attackerFactionComponent.faction, defenderFactionComponent.faction);
                                    bool isDefenderIncapacitated = EntityManager.getIncapacitatedComponent(defenderId) != null;

                                    if (isDefenderWithinRange && isDefenderAttackable && !isDefenderIncapacitated)
                                    {
                                        attack(attackerId, defenderId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Handle ranged attacks
        private void handleRangedAttacks(List<int> attackerEntities, List<int> characterEntities)
        {
            foreach (int attackerId in attackerEntities)
            {
                if (EntityManager.doesEntityExist(attackerId))  // entity could have been killed earlier this frame
                {
                    SkillsComponent attackerSkills = EntityManager.getSkillsComponent(attackerId);
                    RangedAttackSkill attackerRangedAttackSkill = attackerSkills.getSkill(SkillType.RangedAttack) as RangedAttackSkill;

                    if (attackerRangedAttackSkill != null)   // attacker has a ranged skill
                    {
                        PositionComponent attackerPositionComponent = EntityManager.getPositionComponent(attackerId);
                        FactionComponent attackerFactionComponent = EntityManager.getFactionComponent(attackerId);

                        if (attackerRangedAttackSkill.cooldown == 0)   // ready to attack
                        {
                            foreach (int defenderId in characterEntities)
                            {
                                if (EntityManager.doesEntityExist(defenderId))  // entity could have been killed earlier this frame
                                {
                                    PositionComponent defenderPositionComponent = EntityManager.getPositionComponent(defenderId);
                                    FactionComponent defenderFactionComponent = EntityManager.getFactionComponent(defenderId);
                                    Vector2 relative = defenderPositionComponent.position - attackerPositionComponent.position;
                                    bool isDefenderWithinRange = relative.Length() <= (attackerRangedAttackSkill.range + SKILL_RANGE_TOLERANCE);
                                    bool isDefenderAttackable = isFactionAttackable(attackerFactionComponent.faction, defenderFactionComponent.faction);
                                    bool isDefenderIncapacitated = EntityManager.getIncapacitatedComponent(defenderId) != null;

                                    if (isDefenderWithinRange && isDefenderAttackable && !isDefenderIncapacitated)
                                    {
                                        attack(attackerId, defenderId);
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Update system
        public void update()
        {
            List<int> skillsEntities = EntityManager.getEntitiesPossessing(ComponentType.Skills);
            List<int> attackableEntities = EntityManager.getEntitiesPossessing(ComponentType.Stats);

            // Handle skill cooldown timers
            handleSkillCooldowns(skillsEntities);

            // Handle melee attacks
            handleMeleeAttacks(skillsEntities, attackableEntities);

            // Handle ranged attacks
            handleRangedAttacks(skillsEntities, attackableEntities);
        }
    }
}
