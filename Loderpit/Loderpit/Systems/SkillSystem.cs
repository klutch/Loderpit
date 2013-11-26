using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Factories;
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
                        case SkillType.Block:
                            initializeBlockSkill(entityId, skill as BlockSkill);
                            break;
                    }
                }
            }
        }

        // Initialize block skill
        private void initializeBlockSkill(int entityId, BlockSkill skill)
        {
            CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
            Fixture fixture = FixtureFactory.AttachRectangle(1f, 1f, 0.1f, Vector2.Zero, characterComponent.body);

            // Create a 'shield' fixture (attached to the character's body) that only collides with hostile characters
            fixture.OnCollision += new OnCollisionEventHandler((fixtureA, fixtureB, contact) =>
                {
                    int entityIdA = (int)fixtureA.Body.UserData;
                    int entityIdB;
                    FactionComponent factionComponentA = EntityManager.getFactionComponent(entityIdA);
                    FactionComponent factionComponentB;

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

                    Console.WriteLine("colliding with hostile character");

                    return true;
                });
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
                SystemManager.combatSystem.attack(entityId, executePowerShotSkill.defenderId, powerShotSkill.calculateExtraDamage());
            }
            SystemManager.skillSystem.resetCooldown(entityId, SkillType.PowerShot);
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);

            removeExecutedSkill(entityId, executePowerShotSkill);
        }

        // Execute power swing
        private void executePowerSwing(int entityId, ExecutePowerSwingSkill executePowerSwingSkill)
        {
            PerformingSkillsComponent performingSkillsComponent = EntityManager.getPerformingSkillsComponent(entityId);
            PowerSwingSkill powerSwingSkill = executePowerSwingSkill.skill as PowerSwingSkill;

            if (EntityManager.doesEntityExist(executePowerSwingSkill.defenderId))    // defender could have died already
            {
                SystemManager.combatSystem.attack(entityId, executePowerSwingSkill.defenderId, powerSwingSkill.calculateExtraDamage());
            }
            SystemManager.skillSystem.resetCooldown(entityId, SkillType.PowerSwing);
            EntityManager.removeComponent(entityId, ComponentType.PositionTarget);

            removeExecutedSkill(entityId, executePowerSwingSkill);
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

                            // Archer
                            case SkillType.PowerShot:
                                executePowerShot(entityId, executeSkill as ExecutePowerShotSkill);
                                break;

                            // Fighter
                            case SkillType.PowerSwing:
                                executePowerSwing(entityId, executeSkill as ExecutePowerSwingSkill);
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

        // Update
        public void update()
        {
            List<int> skillsEntities = EntityManager.getEntitiesPossessing(ComponentType.Skills);
            List<int> performSkillsEntities = EntityManager.getEntitiesPossessing(ComponentType.PerformingSkills);

            // Decrement skill cooldowns
            decrementSkillCooldowns(skillsEntities);

            // Handle perform skills
            handlePerformSkills(performSkillsEntities);

            // Handle executed skills cleanup
            handleExecutedSkillsCleanup();
        }
    }
}
