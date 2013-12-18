using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SFML.Graphics;
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
        private void addMessage(Vector2 position, string value)
        {
            ScreenManager.levelScreen.addTemporaryWorldText(value, position);
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

        // Apply damage to an entity
        public void applyDamage(int attackerId, int defenderId, int damage)
        {
            int modifiedDamage = damage;
            StatsComponent defenderStats = EntityManager.getStatsComponent(defenderId);
            AffectedBySpellEntitiesComponent defenderSpells = EntityManager.getAffectedBySpellEntitiesComponent(defenderId);
            DamageTransferComponent damageTransferComponent = null;
            DamageMitigationComponent damageMitigationComponent = null;

            // Search for damage transfer spell components
            // TODO: Take into account more than a single damage transfer component?
            foreach (int spellId in defenderSpells.spellEntities)
            {
                damageTransferComponent = damageTransferComponent ?? EntityManager.getDamageTransferComponent(spellId);
                damageMitigationComponent = damageMitigationComponent ?? EntityManager.getDamageMitigationComponent(spellId);
            }

            // Apply damage mitigation
            if (damageMitigationComponent != null)
            {
                modifiedDamage = Math.Max(0, damage - (int)Math.Ceiling((float)damage * damageMitigationComponent.mitigationPercentage));
            }

            // Early exit check
            if (modifiedDamage == 0)
            {
                return;
            }

            if (damageTransferComponent == null)
            {
                // Apply damage normally
                defenderStats.currentHp -= modifiedDamage;
            }
            else
            {
                // Apply damage, transfering some
                StatsComponent guardianStats = EntityManager.getStatsComponent(damageTransferComponent.transferToEntityId);
                int transferedDamage = (int)Math.Ceiling((float)modifiedDamage * damageTransferComponent.transferPercentage);
                int remainingDamage = (int)Math.Floor((float)modifiedDamage - transferedDamage);

                guardianStats.currentHp -= transferedDamage;
                defenderStats.currentHp -= remainingDamage;
            }

            // Check for zero health
            if (defenderStats.currentHp == 0)
            {
                handleZeroHealth(defenderId);
            }
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
            StatSystem statSystem = SystemManager.statSystem;
            StatsComponent defenderStats = EntityManager.getStatsComponent(defenderId);
            AffectedBySpellEntitiesComponent attackerSpells = EntityManager.getAffectedBySpellEntitiesComponent(attackerId);
            AffectedBySpellEntitiesComponent defenderSpells = EntityManager.getAffectedBySpellEntitiesComponent(defenderId);
            PositionComponent defenderPositionComponent = EntityManager.getPositionComponent(defenderId);
            PositionComponent attackerPositionComponent = EntityManager.getPositionComponent(attackerId);
            BloodColorComponent defenderBloodColorComponent = EntityManager.getBloodColorComponent(defenderId);
            RiposteComponent defenderRiposteComponent = null;
            int defenderArmorClass = SystemManager.statSystem.getArmorClass(defenderId);
            int attackRoll;
            Vector2 relative = defenderPositionComponent.position - attackerPositionComponent.position;

            attackDie = attackDie ?? SystemManager.statSystem.getAttackDie(attackerId);
            hitDie = hitDie ?? SystemManager.statSystem.getDamageDie(attackerId);
            attackRoll = Roller.roll(attackDie) + statSystem.getStatModifier(statSystem.getStrength(attackerId));

            // Check defender for riposte spell effect unless attacking skill is a riposte skill (to prevent a possible endless loop)
            if (attackSkill.type != SkillType.Riposte)
            {
                foreach (int spellId in defenderSpells.spellEntities)
                {
                    if ((defenderRiposteComponent = EntityManager.getRiposteComponent(spellId)) != null)
                    {
                        break;
                    }
                }
            }

            // Handle riposte
            if (defenderRiposteComponent != null)
            {
                SkillsComponent defenderSkillsComponent = EntityManager.getSkillsComponent(defenderId);
                RiposteSkill riposteSkill = defenderSkillsComponent.getSkill(SkillType.Riposte) as RiposteSkill;

                if (Roller.roll(defenderRiposteComponent.chanceToRiposte) == 1)
                {
                    attack(riposteSkill, defenderId, attackerId);
                    addMessage(defenderPositionComponent.position, "Riposte");
                    return false;
                }
            }

            // Proceed with normal attack process
            if (attackRoll >= defenderArmorClass)
            {
                // Roll damage
                int hitRoll = Roller.roll(hitDie);
                int damage = hitRoll + extraDamage;

                // Apply damage
                applyDamage(attackerId, defenderId, damage);
                addMessage(defenderPositionComponent.position, "-" + damage.ToString());

                // Create shot trail
                if (attackSkill.type == SkillType.RangedAttack || attackSkill.type == SkillType.Piercing)
                {
                    SystemManager.particleRenderSystem.addShotTrail(Color.White, attackerPositionComponent.position, defenderPositionComponent.position);
                }

                // Blood particle effects
                if (defenderBloodColorComponent != null)
                {
                    SystemManager.particleRenderSystem.addBloodParticleEffect(defenderBloodColorComponent.color, defenderPositionComponent.position, (Vector2.Normalize(relative) + new Vector2(0, -1f)) * 4f, 8);
                }

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

                return true;
            }
            else
            {
                // Miss
                addMessage(defenderPositionComponent.position, "Miss");

                return false;
            }
        }

        // Apply spell damage -- Eventually this could factor in entity resistences
        public void applySpellDamage(int defenderId, int damage)
        {
            StatsComponent defenderStatsComponent = EntityManager.getStatsComponent(defenderId);
            PositionComponent defenderPositionComponent = EntityManager.getPositionComponent(defenderId);

            // Apply damage
            defenderStatsComponent.currentHp -= damage;
            addMessage(defenderPositionComponent.position, "Spell: -" + damage.ToString());

            // Check for zero health
            if (defenderStatsComponent.currentHp == 0)
            {
                handleZeroHealth(defenderId);
            }
        }

        // Apply spell healing
        public void applySpellHeal(int targetId, int amount)
        {
            StatsComponent targetStats = EntityManager.getStatsComponent(targetId);
            PositionComponent targetPositionComponent = EntityManager.getPositionComponent(targetId);

            targetStats.currentHp += amount;
            addMessage(targetPositionComponent.position, "Heal: " + amount.ToString());
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
            IsProxyComponent isProxyComponent = EntityManager.getIsProxyComponent(defenderId);
            List<int> allCombatEntities;

            if (factionComponent.faction == Faction.Player && isProxyComponent == null)
            {
                // Incapacitate the entity
                EntityManager.addComponent(defenderId, new IncapacitatedComponent(defenderId));
            }
            else
            {
                // Kill the entity
                EntityManager.destroyEntity(defenderId);
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

        // Handle attacks
        private void handleAttacks(List<int> attackerEntities, List<int> defenderEntities)
        {
            // Prevent attacks from happening too quickly
            if (SystemManager.physicsSystem.isSlowMotion && !SystemManager.physicsSystem.isReadyForSlowMotionTick)
            {
                return;
            }

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

                        // Skip ranged attacks if the entity has a piercing skill
                        if (skill.type == SkillType.RangedAttack && attackerSkills.getSkill(SkillType.Piercing) != null)
                        {
                            continue;
                        }

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
                                        // Attack differently based on the skill
                                        if (skill.type == SkillType.Piercing)
                                        {
                                            Vector2 normal = Vector2.Normalize(relative);
                                            List<int> targetIds = Helpers.findEntitiesAlongRay(attackerPositionComponent.position, normal, 100f, attackerFactionComponent.attackableFactions, attackerId);

                                            foreach (int targetId in targetIds)
                                            {
                                                attack(skill, attackerId, targetId, 0, null, null);
                                            }
                                        }
                                        else
                                        {
                                            attack(skill, attackerId, defenderId, 0, null, null);
                                        }

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
            // Prevent heals from happening too quickly
            if (SystemManager.physicsSystem.isSlowMotion && !SystemManager.physicsSystem.isReadyForSlowMotionTick)
            {
                return;
            }

            foreach (int entityId in entities)
            {
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
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
                friendlyEntities = Helpers.findEntitiesWithinRange(positionComponent.position, healSkill.range, factionComponent.faction);
                friendlyEntities.Add(entityId); // consider healing self
                foreach (int friendId in friendlyEntities)
                {
                    StatsComponent statsComponent = EntityManager.getStatsComponent(friendId);

                    // If entity is damaged and has less than the lowest hp
                    if (statsComponent.currentHp < SystemManager.statSystem.getMaxHp(friendId) && statsComponent.currentHp < lowestHp)
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
                applySpellHeal(healTargetId, Roller.roll(healSkill.healDie));
                SystemManager.skillSystem.resetCooldown(entityId, SkillType.Heal);
            }
        }

        // Update system
        public void update()
        {
            // Handle attacks
            handleAttacks(EntityManager.getEntitiesPossessing(ComponentType.Skills), EntityManager.getEntitiesPossessing(ComponentType.Stats));

            // Handle heals
            handleHeals(EntityManager.getEntitiesPossessing(ComponentType.Skills));
        }
    }
}
