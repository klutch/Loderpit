﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
using FarseerPhysics.Common;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;
using Loderpit.Formations;

namespace Loderpit.Systems
{
    /*
     * This system handles the execution of all skills in the game. The terminology I'm using is:
     * 
     * Performing a skill -- This means a skill has been supplied with the necessary information and
     *   will be executed once all the prerequisites are met (ie, delay counting down to zero)
     *   
     * Executing a skill -- This means the skill's functionality is now going to be run.
     * 
     * Note: Some skills (like melee/ranged attacks) are performed instantly, so I don't go to the trouble of
     *   creating an execute object for them.
     */
    public class SkillSystem : ISystem
    {
        public const float SKILL_RANGE_TOLERANCE = 0.2f;
        private Dictionary<int, List<ExecuteSkill>> _skillsToRemove;

        public SystemType systemType { get { return SystemType.Skill; } }

        public SkillSystem()
        {
            _skillsToRemove = new Dictionary<int, List<ExecuteSkill>>();
        }

        #region Initialize skill methods

        // Initialize skills -- Perform any logic needed to set up a skill at the beginning of a level
        public void initializeSkills()
        {
            List<int> entities = EntityManager.getEntitiesPossessing(ComponentType.Skills);

            foreach (int entityId in entities)
            {
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);

                foreach (Skill skill in skillsComponent.skills)
                {
                    switch (skill.type)
                    {
                        // Defender
                        case SkillType.Block:
                            initializeBlockSkill(entityId, skill as BlockSkill);
                            break;
                        case SkillType.SpikedShield:
                            initializeSpikedShieldSkill(entityId, skill as SpikedShieldSkill);
                            break;
                        case SkillType.ShieldBash:
                            initializeShieldBashSkill(entityId, skill as ShieldBashSkill);
                            break;

                        // Archer
                        case SkillType.Deadeye:
                            initializeDeadeyeSkill(entityId, skill as DeadeyeSkill);
                            break;
                        case SkillType.ShieldOfThorns:
                            initializeShieldOfThornsSkill(entityId, skill as ShieldOfThornsSkill);
                            break;

                        // Fighter
                        case SkillType.Kick:
                            initializeKickSkill(entityId, skill as KickSkill);
                            break;
                        case SkillType.Bloodletter:
                            initializeBloodletterSkill(entityId, skill as BloodletterSkill);
                            break;

                        // Mage
                        case SkillType.Ignite:
                            initializeIgniteSkill(entityId, skill as IgniteSkill);
                            break;
                    }
                }
            }
        }

        // Initialize block skill
        private void initializeBlockSkill(int entityId, BlockSkill skill)
        {
            // Create a 'shield' fixture (attached to the character's body) that only collides with hostile characters
            CharacterComponent characterComponentA = EntityManager.getCharacterComponent(entityId);
            Body shieldBody = BodyFactory.CreateBody(SystemManager.physicsSystem.world, characterComponentA.body.Position);
            Fixture fixture = FixtureFactory.AttachPolygon(new Vertices(new Vector2[] {
                new Vector2(-1f, -1f),
                new Vector2(0.4f, -1f),
                new Vector2(0.6f, 1f),
                new Vector2(0.4f, 1.5f),
                new Vector2(-1f, 1f)
            }), 1f, shieldBody);

            shieldBody.Friction = 0f;
            shieldBody.UserData = entityId;
            shieldBody.OnCollision += new OnCollisionEventHandler((fixtureA, fixtureB, contact) =>
                {
                    int entityIdA = (int)fixtureA.Body.UserData;
                    int entityIdB;
                    FactionComponent factionComponentA = EntityManager.getFactionComponent(entityIdA);
                    FactionComponent factionComponentB;
                    CharacterComponent characterComponentB;
                    ExternalMovementSpeedsComponent externalSpeedsB;

                    // Skip if not touching -- not sure if this is necessary
                    if (!contact.IsTouching)
                    {
                        return false;
                    }

                    // Skip fixtures whose bodies don't have a userdata
                    if (fixtureB.Body.UserData == null)
                    {
                        return false;
                    }

                    entityIdB = (int)fixtureB.Body.UserData;

                    // Skip if entity doesn't have a faction component
                    if ((factionComponentB = EntityManager.getFactionComponent(entityIdB)) == null)
                    {
                        return false;
                    }

                    // Skip if factions aren't hostile
                    if (factionComponentA.hostileFaction != factionComponentB.faction)
                    {
                        return false;
                    }

                    // Skip if no character component
                    if ((characterComponentB = EntityManager.getCharacterComponent(entityIdB)) == null)
                    {
                        return false;
                    }

                    // Make sure the enemy is in front of the shield
                    if (characterComponentB.body.Position.X <= characterComponentA.body.Position.X)
                    {
                        return false;
                    }

                    // Make sure external speeds component exists
                    if ((externalSpeedsB = EntityManager.getExternalMovementSpeedsComponent(entityIdB)) == null)
                    {
                        return false;
                    }

                    characterComponentB.feet.Friction = 0f;
                    externalSpeedsB.addExternalMovementSpeed(ExternalMovementSpeedType.ShieldBlock, characterComponentA.movementSpeed);

                    return true;
                });

            shieldBody.OnSeparation += new OnSeparationEventHandler((fixtureA, fixtureB) =>
                {
                    int entityIdA = (int)fixtureA.Body.UserData;
                    int entityIdB;
                    FactionComponent factionComponentA = EntityManager.getFactionComponent(entityIdA);
                    FactionComponent factionComponentB;
                    CharacterComponent characterComponentB;
                    ExternalMovementSpeedsComponent externalSpeeds;

                    // Skip fixtures whose bodies don't have a userdata
                    if (fixtureB.Body.UserData == null)
                    {
                        return;
                    }

                    entityIdB = (int)fixtureB.Body.UserData;

                    // Skip if entity doesn't have a faction component
                    if ((factionComponentB = EntityManager.getFactionComponent(entityIdB)) == null)
                    {
                        return;
                    }

                    // Skip if factions aren't hostile
                    if (factionComponentA.hostileFaction != factionComponentB.faction)
                    {
                        return;
                    }

                    // Skip if no character component
                    if ((characterComponentB = EntityManager.getCharacterComponent(entityIdB)) == null)
                    {
                        return;
                    }

                    // Make sure external movement speeds component exists
                    if ((externalSpeeds = EntityManager.getExternalMovementSpeedsComponent(entityIdB)) == null)
                    {
                        return;
                    }

                    characterComponentB.feet.Friction = 5f;
                    characterComponentB.feet.Enabled = false;
                    characterComponentB.feet.Enabled = true;
                    externalSpeeds.removeExternalMovementSpeed(ExternalMovementSpeedType.ShieldBlock);
                });

            fixture.UserData = SpecialFixtureType.Shield;

            EntityManager.addComponent(entityId, new ShieldComponent(entityId, shieldBody));
        }

        // Initialize deadeye skill
        private void initializeDeadeyeSkill(int entityId, DeadeyeSkill deadeyeSkill)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);

            EntityFactory.createDeadeyeSpell(entityId, deadeyeSkill.attackDieMod, deadeyeSkill.range, new List<Faction>(new [] { factionComponent.faction }));
        }

        // Initialize shield of thorns skill
        private void initializeShieldOfThornsSkill(int entityId, ShieldOfThornsSkill shieldOfThornsSkill)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);

            EntityFactory.createShieldOfThornsSpell(entityId, shieldOfThornsSkill.damageDie, shieldOfThornsSkill.range, new List<Faction>( new [] { factionComponent.faction }));
        }

        // Initialize spiked shield skill
        private void initializeSpikedShieldSkill(int entityId, SpikedShieldSkill spikedShieldSkill)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);

            EntityFactory.createSpikedShieldSpell(entityId, spikedShieldSkill.damageDie, spikedShieldSkill.range, new List<Faction>(new [] { factionComponent.hostileFaction }));
        }

        // Initialize kick skill
        private void initializeKickSkill(int entityId, KickSkill kickSkill)
        {
            Action<Skill, int, int> onHitOther = (skill, attackerId, defenderId) =>
                {
                    // Skip if not kick skill
                    if (skill.type != SkillType.Kick)
                    {
                        return;
                    }

                    // Check chance to proc
                    if (Roller.roll(kickSkill.chanceToKnockback) == 1)
                    {
                        PositionComponent attackerPosition = EntityManager.getPositionComponent(attackerId);
                        PositionComponent defenderPosition = EntityManager.getPositionComponent(defenderId);
                        Vector2 normal = attackerPosition.position.X < defenderPosition.position.X ? new Vector2(1f, -0.5f) : new Vector2(-1f, -0.5f);

                        SystemManager.combatSystem.applyKnockback(attackerId, defenderId, kickSkill.knockbackForce, normal);
                    }
                };

            EntityFactory.createProcSpell(entityId, onHitOther);
        }

        // Initialize bloodletter skill
        private void initializeBloodletterSkill(int entityId, BloodletterSkill bloodletterSkill)
        {
            Action<Skill, int, int> onHitOther = (skill, attackerId, defenderId) =>
                {
                    // Only proc on normal attacks
                    if (skill.type != SkillType.MeleeAttack)
                    {
                        return;
                    }

                    // Check chance to proc
                    if (Roller.roll(bloodletterSkill.chanceToProc) == 1)
                    {
                        EntityFactory.createDoTSpell(defenderId, bloodletterSkill.bleedingDamageDie, bloodletterSkill.tickDelay, bloodletterSkill.tickCount);
                    }
                };

            EntityFactory.createProcSpell(entityId, onHitOther);
        }

        // Initialize ignite skill
        private void initializeIgniteSkill(int entityId, IgniteSkill igniteSkill)
        {
            Action<Skill, int, int> onHitOther = (skill, attackerId, defenderId) =>
                {
                    // Skip if not ignite skill
                    if (skill.type != SkillType.RangedAttack)
                    {
                        return;
                    }

                    // Check chance to proc
                    if (Roller.roll(igniteSkill.chanceToProc) == 1)
                    {
                        EntityFactory.createDoTSpell(defenderId, igniteSkill.damageDie, igniteSkill.tickDelay, igniteSkill.tickCount);
                    }
                };

            EntityFactory.createProcSpell(entityId, onHitOther);
        }

        // Initialize shield bash skill
        private void initializeShieldBashSkill(int entityId, ShieldBashSkill shieldBashSkill)
        {
            Action<Skill, int, int> onHitOther = (skill, attackerId, defenderId) =>
            {
                // Skip if not shield bash skill
                if (skill.type != SkillType.ShieldBash)
                {
                    return;
                }

                SystemManager.combatSystem.applyKnockback(attackerId, defenderId, shieldBashSkill.knockbackForce, shieldBashSkill.knockbackNormal);
            };

            EntityFactory.createProcSpell(entityId, onHitOther);
        }

        #endregion


        #region Perform skill methods

        // Perform melee attack skill -- Doesn't create an execute object, because it happens instantly
        public void performMeleeAttackSkill(int entityId, MeleeAttackSkill meleeAttackSkill, Vector2 position)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(Game.worldMouse);

            foreach (Fixture fixture in fixtures)
            {
                int targetEntityId;
                FactionComponent targetFactionComponent;

                // Skip bodies without a user data
                if (fixture.Body.UserData == null)
                {
                    continue;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip entities without a faction component
                if ((targetFactionComponent = EntityManager.getFactionComponent(targetEntityId)) == null)
                {
                    continue;
                }

                // Skip over non-attackable entities
                if (!SystemManager.combatSystem.isFactionAttackable(factionComponent.faction, targetFactionComponent.faction))
                {
                    continue;
                }

                SystemManager.combatSystem.startActiveAttack(entityId, targetEntityId, meleeAttackSkill.range);
                break;
            }
        }

        // Perform ranged attack skill -- Doesn't create an execute object, because it happens instantly
        public void performRangedAttackSkill(int entityId, RangedAttackSkill rangedAttackSkill, Vector2 position)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(Game.worldMouse);

            foreach (Fixture fixture in fixtures)
            {
                int targetEntityId;
                FactionComponent targetFactionComponent;

                // Skip bodies without a user data
                if (fixture.Body.UserData == null)
                {
                    continue;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip entities without a faction component
                if ((targetFactionComponent = EntityManager.getFactionComponent(targetEntityId)) == null)
                {
                    continue;
                }

                // Skip over non-attackable entities
                if (!SystemManager.combatSystem.isFactionAttackable(factionComponent.faction, targetFactionComponent.faction))
                {
                    continue;
                }

                SystemManager.combatSystem.startActiveAttack(entityId, targetEntityId, rangedAttackSkill.range);
                break;
            }
        }

        // Perform build bridge skill
        public void performBuildBridgeSkill(int entityId, BuildBridgeSkill buildBridgeSkill, Vector2 anchorA, Vector2 anchorB)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            ExecuteBuildBridgeSkill executeSkill;
            GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);
            PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
            float distanceA = Math.Abs(anchorA.X - positionComponent.position.X);
            float distanceB = Math.Abs(anchorB.X - positionComponent.position.X);
            Vector2 closestAnchor = distanceA > distanceB ? anchorB : anchorA;

            // Create execute skill object
            executeSkill = new ExecuteBuildBridgeSkill(
                buildBridgeSkill,
                anchorA,
                anchorB,
                () =>
                {
                    PositionComponent futurePositionComponent = EntityManager.getPositionComponent(entityId);
                    PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                    float distance = Math.Abs(positionTargetComponent.position - futurePositionComponent.position.X);

                    return distance <= positionTargetComponent.tolerance;
                });

            if (groupComponent != null)
            {
                LimitedRangeFormation formation = new LimitedRangeFormation(groupComponent.entities, groupComponent.activeFormation.position, groupComponent.activeFormation.speed, float.MinValue, closestAnchor.X - 2f);

                executeSkill.formationToRemove = formation;
                groupComponent.addFormation(formation);
            }

            EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, closestAnchor.X, buildBridgeSkill.range));
            performingSkillsComponent.executingSkills.Add(executeSkill);
        }

        // Perform throw rope skill
        public void performThrowRopeSkill(int entityId, ThrowRopeSkill throwRopeSkill, Vector2 anchor)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);
            ExecuteThrowRopeSkill executeSkill;

            // Create execute skill object
            executeSkill = new ExecuteThrowRopeSkill(
                throwRopeSkill,
                anchor,
                () =>
                {
                    PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                    PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                    float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);

                    return distance <= positionTargetComponent.tolerance;
                });

            // Check for a group
            if (groupComponent != null)
            {
                LimitedRangeFormation formation = new LimitedRangeFormation(groupComponent.entities, groupComponent.activeFormation.position, groupComponent.activeFormation.speed, float.MinValue, anchor.X - 4f);

                groupComponent.addFormation(formation);
                executeSkill.formationToRemove = formation;
            }

            EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, anchor.X, throwRopeSkill.range));
            performingSkillsComponent.executingSkills.Add(executeSkill);
        }

        // Perform power shot skill
        public void performPowerShotSkill(int entityId, PowerShotSkill powerShotSkill, Vector2 target)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            ExecutePowerShotSkill executePowerShotSkill = null;
            List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(target);

            foreach (Fixture fixture in fixtures)
            {
                int targetEntityId;
                FactionComponent targetFactionComponent;

                // Skip bodies without any userdata
                if (fixture.Body.UserData == null)
                {
                    continue;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip entities without a faction component
                if ((targetFactionComponent = EntityManager.getFactionComponent(targetEntityId)) == null)
                {
                    continue;
                }

                // Skip over non-attackable entities
                if (!SystemManager.combatSystem.isFactionAttackable(factionComponent.faction, targetFactionComponent.faction))
                {
                    continue;
                }

                // Create execute skill object
                executePowerShotSkill = new ExecutePowerShotSkill(
                    powerShotSkill,
                    targetEntityId,
                    () =>
                    {
                        PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                        PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                        float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);

                        return distance <= positionTargetComponent.tolerance;
                    });

                EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, fixture.Body, powerShotSkill.range));
                break;
            }

            if (executePowerShotSkill != null)
            {
                performingSkillsComponent.executingSkills.Add(executePowerShotSkill);
            }
        }

        // Perform power swing skill
        public void performPowerSwingSkill(int entityId, PowerSwingSkill powerSwingSkill, Vector2 target)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            ExecutePowerSwingSkill executePowerSwingSkill = null;
            List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(target);

            foreach (Fixture fixture in fixtures)
            {
                int targetEntityId;
                FactionComponent targetFactionComponent;

                // Skip bodies without any userdata
                if (fixture.Body.UserData == null)
                {
                    continue;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip entities without a faction component
                if ((targetFactionComponent = EntityManager.getFactionComponent(targetEntityId)) == null)
                {
                    continue;
                }

                // Skip over non-attackable entities
                if (!SystemManager.combatSystem.isFactionAttackable(factionComponent.faction, targetFactionComponent.faction))
                {
                    continue;
                }

                // Create execute skill object
                executePowerSwingSkill = new ExecutePowerSwingSkill(
                    powerSwingSkill,
                    targetEntityId,
                    () =>
                    {
                        PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                        PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                        float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);

                        return distance <= positionTargetComponent.tolerance;
                    });

                EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, fixture.Body, powerSwingSkill.range));
                break;
            }

            if (executePowerSwingSkill != null)
            {
                performingSkillsComponent.executingSkills.Add(executePowerSwingSkill);
            }
        }

        // Perform fireball skill
        public void performFireballSkill(int entityId, FireballSkill fireballSkill, Vector2 target)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            ExecuteFireballSkill executeFireballSkill = new ExecuteFireballSkill(
                fireballSkill,
                target,
                () =>
                {
                    PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                    PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                    float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);

                    return distance <= positionTargetComponent.tolerance;
                });

            performingSkillsComponent.executingSkills.Add(executeFireballSkill);
            EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, target.X, fireballSkill.range));
        }

        // Perform shield bash skill
        public void performShieldBashSkill(int entityId, ShieldBashSkill shieldBashSkill)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
            List<int> affectedEntities = Helpers.findEntitiesWithinRange(positionComponent.position, 3f, factionComponent.hostileFaction, entityId);

            foreach (int affectedId in affectedEntities)
            {
                SystemManager.combatSystem.attack(shieldBashSkill, entityId, affectedId, 0, shieldBashSkill.attackDie, shieldBashSkill.damageDie);
                resetCooldown(entityId, SkillType.ShieldBash);
            }
        }

        // Perform healing blast
        public void performHealingBlastSkill(int entityId, HealingBlastSkill healingBlastSkill, Vector2 target)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            ExecuteHealingBlastSkill executeSkill = null;
            List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(target);

            foreach (Fixture fixture in fixtures)
            {
                int targetEntityId;
                FactionComponent targetFactionComponent;

                // Skip bodies without any userdata
                if (fixture.Body.UserData == null)
                {
                    continue;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip entities without a faction component
                if ((targetFactionComponent = EntityManager.getFactionComponent(targetEntityId)) == null)
                {
                    continue;
                }

                // Skip over hostile entities
                if (factionComponent.faction != targetFactionComponent.faction)
                {
                    continue;
                }

                // Create execute skill object
                executeSkill = new ExecuteHealingBlastSkill(
                    healingBlastSkill,
                    targetEntityId,
                    () =>
                    {
                        PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                        PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                        float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);

                        return distance <= positionTargetComponent.tolerance;
                    });

                EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, fixture.Body, healingBlastSkill.range));
                break;
            }

            if (executeSkill != null)
            {
                performingSkillsComponent.executingSkills.Add(executeSkill);
            }
        }

        // Perform proximity mine skill
        public void performProximityMineSkill(int entityId, ProximityMineSkill skill, Vector2 target)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);

            // Create execute skill object
            performingSkillsComponent.executingSkills.Add(new ExecuteProximityMineSkill(
                skill,
                target,
                () =>
                {
                    PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                    PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                    float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);

                    return distance <= positionTargetComponent.tolerance;
                }));

            EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, target.X, skill.range));
        }

        // Perform fatality skill
        public void performFatalitySkill(int entityId, FatalitySkill skill, Vector2 target)
        {
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            ExecuteFatalitySkill executeSkill = null;
            List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(target);

            foreach (Fixture fixture in fixtures)
            {
                int targetEntityId;
                FactionComponent targetFactionComponent;

                // Skip bodies without any userdata
                if (fixture.Body.UserData == null)
                {
                    continue;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip entities without a faction component
                if ((targetFactionComponent = EntityManager.getFactionComponent(targetEntityId)) == null)
                {
                    continue;
                }

                // Skip over non-attackable entities
                if (!SystemManager.combatSystem.isFactionAttackable(factionComponent.faction, targetFactionComponent.faction))
                {
                    continue;
                }

                // Create execute skill object
                executeSkill = new ExecuteFatalitySkill(
                    skill,
                    targetEntityId,
                    () =>
                    {
                        PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                        PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                        float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);

                        return distance <= positionTargetComponent.tolerance;
                    });

                EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, fixture.Body, skill.range));
                break;
            }

            if (executeSkill != null)
            {
                performingSkillsComponent.executingSkills.Add(executeSkill);
            }
        }

        // Perform infusion skill
        public void performInfusionSkill(int entityId, InfusionSkill infusionSkill, int targetEntityId)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            CharacterComponent characterComponent = EntityManager.getCharacterComponent(targetEntityId);
            bool inRangeAtleastOnce = false;
            ExecuteInfusionSkill executeSkill = new ExecuteInfusionSkill(infusionSkill, targetEntityId, () =>
                {
                    if (inRangeAtleastOnce)
                    {
                        return true;
                    }
                    else
                    {
                        PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
                        PositionTargetComponent positionTargetComponent = EntityManager.getPositionTargetComponent(entityId);
                        float distance = Math.Abs(positionTargetComponent.position - positionComponent.position.X);
                        bool inRange = distance <= positionTargetComponent.tolerance;

                        if (inRange)
                        {
                            inRangeAtleastOnce = true;
                        }

                        return inRange;
                    }
                });

            EntityManager.addComponent(entityId, new PositionTargetComponent(entityId, characterComponent.body, infusionSkill.range - 2f));
            performingSkillsComponent.executingSkills.Add(executeSkill);
        }

        #endregion

        #region Cooldown methods

        // Reset skill cooldown
        public void resetCooldown(int entityId, SkillType skillType)
        {
            SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
            StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
            Skill skill = skillsComponent.getSkill(skillType);

            switch (skill.type)
            {
                case SkillType.MeleeAttack:
                case SkillType.RangedAttack:
                    skill.setCooldown(statsComponent.attackDelay);
                    break;

                default:
                    skill.setCooldown(skill.calculateBaseCooldown());
                    break;
            }
        }

        // Decrement skill cooldowns
        private void decrementSkillCooldowns(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                StatsComponent statsComponent = EntityManager.getStatsComponent(entityId);
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);

                foreach (Skill skill in skillsComponent.skills)
                {
                    if (skill.cooldown > 0)
                    {
                        skill.decrementCooldown();
                    }
                }
            }
        }

        #endregion

        #region Execute skill methods

        // Execute build bridge
        private void executeBuildBridge(int entityId, ExecuteBuildBridgeSkill executeBuildBridgeSkill)
        {
            PerformingSkillsComponent performSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);

            EntityFactory.createBridge(executeBuildBridgeSkill.anchorA, executeBuildBridgeSkill.anchorB);
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);

            if (groupComponent != null)
            {
                // Update the default formation position to the limited range formation's position, then remove the limited range formation.
                float currentPosition = groupComponent.getFormation(FormationType.LimitedRange).position;

                groupComponent.removeFormation(executeBuildBridgeSkill.formationToRemove);
                groupComponent.getFormation(FormationType.Default).position = currentPosition;
            }

            removeExecutedSkill(entityId, executeBuildBridgeSkill);
        }

        // Execute throw rope
        private void executeThrowRope(int entityId, ExecuteThrowRopeSkill executeThrowRopeSkill)
        {
            PerformingSkillsComponent performSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityId);

            EntityFactory.createRope(executeThrowRopeSkill.anchor);
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);

            if (groupComponent != null)
            {
                float currentPosition = groupComponent.getFormation(FormationType.LimitedRange).position;

                groupComponent.getFormation(FormationType.Default).position = currentPosition;
                groupComponent.removeFormation(executeThrowRopeSkill.formationToRemove);
            }

            removeExecutedSkill(entityId, executeThrowRopeSkill);
        }

        // Execute power shot
        private void executePowerShot(int entityId, ExecutePowerShotSkill executePowerShotSkill)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            PowerShotSkill powerShotSkill = executePowerShotSkill.skill as PowerShotSkill;

            if (EntityManager.doesEntityExist(executePowerShotSkill.defenderId))    // defender could have died already
            {
                SystemManager.combatSystem.attack(powerShotSkill, entityId, executePowerShotSkill.defenderId, powerShotSkill.calculateExtraDamage());
            }
            
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
            resetCooldown(entityId, SkillType.PowerShot);
            removeExecutedSkill(entityId, executePowerShotSkill);
        }

        // Execute power swing
        private void executePowerSwing(int entityId, ExecutePowerSwingSkill executePowerSwingSkill)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            PowerSwingSkill powerSwingSkill = executePowerSwingSkill.skill as PowerSwingSkill;

            if (EntityManager.doesEntityExist(executePowerSwingSkill.defenderId))    // defender could have died already
            {
                SystemManager.combatSystem.attack(powerSwingSkill, entityId, executePowerSwingSkill.defenderId, powerSwingSkill.calculateExtraDamage());
            }
            
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
            resetCooldown(entityId, SkillType.PowerSwing);
            removeExecutedSkill(entityId, executePowerSwingSkill);
        }

        // Execute fatality
        private void executeFatality(int entityId, ExecuteFatalitySkill executeSkill)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            FatalitySkill fatalitySkill = executeSkill.skill as FatalitySkill;

            if (EntityManager.doesEntityExist(executeSkill.targetEntityId))    // defender could have died already
            {
                SystemManager.combatSystem.attack(fatalitySkill, entityId, executeSkill.targetEntityId, 0, "d1+9998", "d1+9998");
            }
            
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
            resetCooldown(entityId, SkillType.Fatality);
            removeExecutedSkill(entityId, executeSkill);
        }

        // Execute fireball
        private void executeFireball(int entityId, ExecuteFireballSkill executeFireballSkill)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            FireballSkill fireballSkill = executeFireballSkill.skill as FireballSkill;
            List<int> hitEntities = SystemManager.explosionSystem.createExplosion(entityId, executeFireballSkill.target, fireballSkill.explosionRadius, Roller.roll(fireballSkill.explosionDamageDie), fireballSkill.explosionForce);

            foreach (int hitEntityId in hitEntities)
            {
                if (EntityManager.doesEntityExist(hitEntityId))
                {
                    if (Roller.roll(fireballSkill.burnChanceDie) == 1)
                    {
                        EntityFactory.createDoTSpell(hitEntityId, fireballSkill.burnDamageDie, fireballSkill.burnTickDelay, fireballSkill.burnTickCount);
                    }
                }
            }

            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
            resetCooldown(entityId, SkillType.Fireball);
            removeExecutedSkill(entityId, executeFireballSkill);
        }

        // Execute healing blast
        private void executeHealingBlast(int entityId, ExecuteHealingBlastSkill executeHealingBlast)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            HealingBlastSkill healingBlastSkill = executeHealingBlast.skill as HealingBlastSkill;
            int targetId = executeHealingBlast.targetId;

            SystemManager.combatSystem.applySpellHeal(entityId, targetId, Roller.roll(healingBlastSkill.healDie));
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
            resetCooldown(entityId, SkillType.HealingBlast);
            removeExecutedSkill(entityId, executeHealingBlast);
        }

        // Execute proximity mine
        private void executeProximityMine(int entityIdA, ExecuteProximityMineSkill executeSkill)
        {
            PositionComponent positionComponent = EntityManager.getPositionComponent(entityIdA);
            FactionComponent factionComponent = EntityManager.getFactionComponent(entityIdA);
            ProximityMineSkill skill = executeSkill.skill as ProximityMineSkill;
            bool hit = false;
            Vector2 point = Vector2.Zero;

            // Cast upwards and check for ceiling
            SystemManager.physicsSystem.world.RayCast((f, p, n, fr) =>
                {
                    int entityIdB;
                    CeilingComponent ceilingComponent;

                    // Skip if fixture doesn't have userdata
                    if (f.Body.UserData == null)
                    {
                        return -1;
                    }

                    entityIdB = (int)f.Body.UserData;

                    // Skip entites without a ceiling component
                    if ((ceilingComponent = EntityManager.getCeilingComponent(entityIdB)) == null)
                    {
                        return -1;
                    }

                    // Hit
                    hit = true;
                    point = p;
                    return fr;
                },
                executeSkill.target + new Vector2(0f, 1000f),
                executeSkill.target + new Vector2(0f, -5f));

            if (!hit)
            {
                // Cast downwards and check for ground
                SystemManager.physicsSystem.world.RayCast((f, p, n, fr) =>
                {
                    int entityIdB;
                    GroundBodyComponent groundComponent;

                    // Skip if fixture doesn't have userdata
                    if (f.Body.UserData == null)
                    {
                        return -1;
                    }

                    entityIdB = (int)f.Body.UserData;

                    // Skip entites without a ground component
                    if ((groundComponent = EntityManager.getGroundBodyComponent(entityIdB)) == null)
                    {
                        return -1;
                    }

                    // Hit
                    hit = true;
                    point = p;
                    return fr;
                },
                executeSkill.target + new Vector2(0f, -1000f),
                executeSkill.target + new Vector2(0f, 5f));
            }

            if (hit)
            {
                EntityFactory.createProximityMine(point, factionComponent.hostileFaction, skill.explosionRadius, skill.explosionForce, skill.damageDie);
            }

            EntityManager.removeComponent(entityIdA, ComponentType.PositionTarget);
            resetCooldown(entityIdA, SkillType.ProximityMine);
            removeExecutedSkill(entityIdA, executeSkill);
        }

        // Execute infusion skill
        private void executeInfusion(int entityId, ExecuteInfusionSkill executeSkill)
        {
            InfusionSkill skill = executeSkill.skill as InfusionSkill;

            EntityFactory.createInfusionSpell(executeSkill.targetEntityId, skill.maxHpMod, skill.strengthMod, skill.armorClassMod, skill.timeToLive);
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);
            resetCooldown(entityId, SkillType.Infusion);
            removeExecutedSkill(entityId, executeSkill);
        }

        #endregion

        // Remove executed action from a PerformSkillsComponent
        private void removeExecutedSkill(int entityId, ExecuteSkill executeSkill)
        {
            if (!_skillsToRemove.ContainsKey(entityId))
            {
                _skillsToRemove.Add(entityId, new List<ExecuteSkill>());
            }

            _skillsToRemove[entityId].Add(executeSkill);
        }

        // Handle perform skills components
        private void handlePerformSkills(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                PerformingSkillsComponent performSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);

                foreach (ExecuteSkill executeSkill in performSkillsComponent.executingSkills)
                {
                    if (executeSkill.delay > 0)
                    {
                        if (executeSkill.isDelayConditionMet())
                        {
                            executeSkill.delay--;
                        }
                    }
                    else
                    {
                        switch (executeSkill.skill.type)
                        {
                            // Engineer
                            case SkillType.BuildBridge:
                                executeBuildBridge(entityId, executeSkill as ExecuteBuildBridgeSkill);
                                break;
                            case SkillType.ThrowRope:
                                executeThrowRope(entityId, executeSkill as ExecuteThrowRopeSkill);
                                break;
                            case SkillType.ProximityMine:
                                executeProximityMine(entityId, executeSkill as ExecuteProximityMineSkill);
                                break;

                            // Archer
                            case SkillType.PowerShot:
                                executePowerShot(entityId, executeSkill as ExecutePowerShotSkill);
                                break;

                            // Fighter
                            case SkillType.PowerSwing:
                                executePowerSwing(entityId, executeSkill as ExecutePowerSwingSkill);
                                break;
                            case SkillType.Fatality:
                                executeFatality(entityId, executeSkill as ExecuteFatalitySkill);
                                break;

                            // Mage
                            case SkillType.Fireball:
                                executeFireball(entityId, executeSkill as ExecuteFireballSkill);
                                break;

                            // Healer
                            case SkillType.HealingBlast:
                                executeHealingBlast(entityId, executeSkill as ExecuteHealingBlastSkill);
                                break;
                            case SkillType.Infusion:
                                executeInfusion(entityId, executeSkill as ExecuteInfusionSkill);
                                break;
                        }
                    }
                }
            }
        }

        // Handle executed skills cleanup
        private void handleExecutedSkillsCleanup()
        {
            foreach (KeyValuePair<int, List<ExecuteSkill>> entityExecutedSkillsPair in _skillsToRemove)
            {
                int entityId = entityExecutedSkillsPair.Key;
                List<ExecuteSkill> executedSkills = entityExecutedSkillsPair.Value;
                PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);

                foreach (ExecuteSkill executedSkill in executedSkills)
                {
                    performingSkillsComponent.executingSkills.Remove(executedSkill);
                }
            }
            _skillsToRemove.Clear();
        }

        // Move shield bodies
        private void moveShieldBodies(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                ShieldComponent shieldComponent = EntityManager.getShieldComponent(entityId);
                PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);

                shieldComponent.body.Position = positionComponent.position;
            }
        }

        // Update
        public void update()
        {
            List<int> skillsEntities = EntityManager.getEntitiesPossessing(ComponentType.Skills);
            List<int> performSkillsEntities = EntityManager.getEntitiesPossessing(ComponentType.PerformingSkills);
            List<int> shieldEntities = EntityManager.getEntitiesPossessing(ComponentType.Shield);

            // Decrement skill cooldowns
            decrementSkillCooldowns(skillsEntities);

            // Handle perform skills
            handlePerformSkills(performSkillsEntities);

            // Handle executed skills cleanup
            handleExecutedSkillsCleanup();

            // Move shield bodies
            moveShieldBodies(shieldEntities);
        }
    }
}
