using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Loderpit.Components;
using Loderpit.Components.SpellEffects;
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
         *  
         * Returns true if the attack was a success, and false if it was a miss
         */
        public bool attack(
            Skill attackSkill,
            int attackerId,
            int defenderId,
            int extraDamage = 0,    // TODO: -- this parameter could be co-opted into a damage die
            string attackDie = null,
            string hitDie = null)
        {
            StatsComponent attackerStats = EntityManager.getStatsComponent(attackerId);
            StatsComponent defenderStats = EntityManager.getStatsComponent(defenderId);
            AffectedBySpellEntitiesComponent attackerSpells = EntityManager.getAffectedBySpellEntitiesComponent(attackerId);
            AffectedBySpellEntitiesComponent defenderSpells = EntityManager.getAffectedBySpellEntitiesComponent(defenderId);
            int defenderArmorClass = SystemManager.statSystem.getArmorClass(defenderId);
            int attackRoll;

            attackDie = attackDie ?? SystemManager.statSystem.getAttackDie(attackerId);
            hitDie = hitDie ?? SystemManager.statSystem.getHitDie(attackerId);
            attackRoll = Roller.roll(attackDie) + SystemManager.statSystem.getStatModifier(attackerStats.strength);

            if (attackRoll >= defenderArmorClass)
            {
                // Hit
                int hitRoll = Roller.roll(hitDie);
                int damage = hitRoll + extraDamage;

                defenderStats.currentHp -= damage;
                addMessage(defenderId, "-" + damage.ToString());

                // Check for attacker procs
                foreach (int spellId in attackerSpells.spellEntities)
                {
                    ProcComponent procComponent = EntityManager.getProcComponent(spellId);

                    if (procComponent != null)
                    {
                        if (procComponent.onHitOther != null)
                        {
                            procComponent.onHitOther(attackSkill, attackerId, defenderId);
                        }
                    }
                }

                // Check for defender procs
                foreach (int spellId in defenderSpells.spellEntities)
                {
                    ProcComponent procComponent = EntityManager.getProcComponent(spellId);

                    if (procComponent != null)
                    {
                        if (procComponent.onHitByOther != null)
                        {
                            procComponent.onHitByOther(attackSkill, attackerId, defenderId);
                        }
                    }
                }

                // Check defender for damage shields
                foreach (int spellId in defenderSpells.spellEntities)
                {
                    DamageShieldComponent damageShieldComponent = EntityManager.getDamageShieldComponent(spellId);

                    if (damageShieldComponent != null)
                    {
                        // Apply damage shield damage
                        applySpellDamage(attackerId, Roller.roll(damageShieldComponent.damageDie));
                    }
                }

                // Check for zero health
                if (defenderStats.currentHp == 0)
                {
                    handleZeroHealth(defenderId);
                }

                return true;
            }
            else
            {
                // Miss
                addMessage(defenderId, "Miss");

                return false;
            }
        }

        // Apply spell damage -- Eventually this could factor in entity resistences
        public void applySpellDamage(int defenderId, int damage)
        {
            StatsComponent defenderStatsComponent = EntityManager.getStatsComponent(defenderId);

            // Apply damage
            defenderStatsComponent.currentHp -= damage;
            addMessage(defenderId, "Spell: -" + damage.ToString());

            // Check for zero health
            if (defenderStatsComponent.currentHp == 0)
            {
                handleZeroHealth(defenderId);
            }
        }

        // Apply spell healing
        public void applySpellHeal(int healerId, int targetId, int amount)
        {
            StatsComponent targetStats = EntityManager.getStatsComponent(targetId);

            targetStats.currentHp += amount;
            addMessage(targetId, "Heal: " + amount.ToString());
        }

        // Apply knockback
        public void applyKnockback(int attackerId, int defenderId, float strength, Vector2 normal)
        {
            PositionComponent attackerPositionComponent = EntityManager.getPositionComponent(attackerId);
            PositionComponent defenderPositionComponent = EntityManager.getPositionComponent(defenderId);
            CharacterComponent defenderCharacterComponent = EntityManager.getCharacterComponent(defenderId);
            Vector2 force = normal * strength;

            // TODO: Make this operate on some type of PhysicsBodyComponent?
            if (defenderCharacterComponent != null)
            {
                defenderCharacterComponent.body.ApplyForce(ref force);
            }
        }

        // Handle an entity with zero health
        private void handleZeroHealth(int defenderId)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(defenderId);
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

        // Handle attacks
        private void handleAttacks(List<int> attackerEntities, List<int> defenderEntities)
        {
            foreach (int attackerId in attackerEntities)
            {
                if (EntityManager.doesEntityExist(attackerId))  // entity could have been killed earlier this frame
                {
                    SkillsComponent attackerSkills = EntityManager.getSkillsComponent(attackerId);
                    List<Skill> attackerAttackSkills = attackerSkills.attackSkills;

                    foreach (Skill skill in attackerAttackSkills)
                    {
                        PositionComponent attackerPositionComponent = EntityManager.getPositionComponent(attackerId);
                        FactionComponent attackerFactionComponent = EntityManager.getFactionComponent(attackerId);

                        if (skill.cooldown == 0)   // ready to attack
                        {
                            foreach (int defenderId in defenderEntities)
                            {
                                if (EntityManager.doesEntityExist(attackerId) && EntityManager.doesEntityExist(defenderId))  // entities could have been killed earlier this frame
                                {
                                    PositionComponent defenderPositionComponent = EntityManager.getPositionComponent(defenderId);
                                    FactionComponent defenderFactionComponent = EntityManager.getFactionComponent(defenderId);
                                    Vector2 relative = defenderPositionComponent.position - attackerPositionComponent.position;
                                    bool isDefenderWithinRange = relative.Length() <= (skill.range + SkillSystem.SKILL_RANGE_TOLERANCE);
                                    bool isDefenderAttackable = isFactionAttackable(attackerFactionComponent.faction, defenderFactionComponent.faction);
                                    bool isDefenderIncapacitated = EntityManager.getIncapacitatedComponent(defenderId) != null;

                                    if (isDefenderWithinRange && isDefenderAttackable && !isDefenderIncapacitated)
                                    {
                                        attack(skill, attackerId, defenderId, 0, null, null);

                                        if (EntityManager.doesEntityExist(attackerId))  // attacker could have been killed by a damage shield
                                        {
                                            SystemManager.skillSystem.resetCooldown(attackerId, skill.type);
                                        }

                                        break;  // prevent attacking multiple entities
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        // Handle (passive) heals
        private void handleHeals(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                HealSkill healSkill;
                List<int> friendlyEntities;
                int healTargetId = 0;
                int lowestHp = int.MaxValue;
                bool foundHealTarget = false;

                // Skip if entity is already dead (possible, since this happens after attacks have been handled)
                if (!EntityManager.doesEntityExist(entityId))
                {
                    continue;
                }

                // Skip if no heal skill
                if ((healSkill = skillsComponent.getSkill(SkillType.Heal) as HealSkill) == null)
                {
                    continue;
                }

                // Skip if cooldown is not zero
                if (healSkill.cooldown != 0)
                {
                    continue;
                }

                // Find target with the lowest hp
                friendlyEntities = Helpers.findEntitiesWithinRange(entityId, healSkill.range, factionComponent.faction);
                friendlyEntities.Add(entityId); // consider healing self
                foreach (int friendId in friendlyEntities)
                {
                    StatsComponent statsComponent = EntityManager.getStatsComponent(friendId);

                    // If entity is damaged and has less than the lowest hp
                    if (statsComponent.currentHp < SystemManager.statSystem.getMaxHp(statsComponent) && statsComponent.currentHp < lowestHp)
                    {
                        foundHealTarget = true;
                        healTargetId = friendId;
                    }
                }

                // Skip if no suitable heal target was found
                if (!foundHealTarget)
                {
                    continue;
                }

                // Heal target
                applySpellHeal(entityId, healTargetId, Roller.roll(healSkill.healDie));
            }
        }

        // Update system
        public void update()
        {
            List<int> skillsEntities = EntityManager.getEntitiesPossessing(ComponentType.Skills);
            List<int> attackableEntities = EntityManager.getEntitiesPossessing(ComponentType.Stats);

            // Handle attacks
            handleAttacks(skillsEntities, attackableEntities);

            // Handle heals
            handleHeals(skillsEntities);
        }
    }
}
