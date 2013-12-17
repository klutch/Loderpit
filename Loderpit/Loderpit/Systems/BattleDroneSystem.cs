using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;

namespace Loderpit.Systems
{
    public class BattleDroneSystem : ISystem
    {
        private Random _rng;

        public SystemType systemType { get { return SystemType.BattleDrone; } }

        public BattleDroneSystem()
        {
            _rng = new Random();
        }

        // Handle spawning battle drones
        private void handleSpawningBattleDrones(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);
                BattleDroneOwnerComponent battleDroneOwnerComponent = EntityManager.getBattleDroneOwnerComponent(entityId);
                BattleDroneSkill battleDroneSkill = skillsComponent.getSkill(SkillType.BattleDrone) as BattleDroneSkill;

                // Skip if all drones are spawned
                if (battleDroneOwnerComponent.droneIds.Count == battleDroneSkill.droneCount)
                {
                    continue;
                }

                // Skip if cooldown not met
                if (battleDroneSkill.cooldown != 0)
                {
                    continue;
                }

                EntityFactory.createBattleDrone(entityId, battleDroneSkill.damageDie, battleDroneSkill.maxHp);
            }
        }

        // Handle follow owners
        private void handleFollowOwners(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                BattleDroneOwnerComponent battleDroneOwnerComponent = EntityManager.getBattleDroneOwnerComponent(entityId);
                PositionComponent ownerPositionComponent = EntityManager.getPositionComponent(entityId);
                Vector2 targetPosition = ownerPositionComponent.position + new Vector2(0, -1f);

                foreach (int droneId in battleDroneOwnerComponent.droneIds)
                {
                    PhysicsComponent dronePhysicsComponent = EntityManager.getPhysicsComponent(droneId);
                    PositionComponent dronePositionComponent = EntityManager.getPositionComponent(droneId);
                    Vector2 relative = targetPosition - dronePositionComponent.position;
                    float length = relative.Length();

                    if (length > 2f)
                    {
                        dronePhysicsComponent.bodies[0].ApplyForce(relative * 25f);
                    }
                    else if (length > 1f)
                    {
                        dronePhysicsComponent.bodies[0].ApplyForce(relative * 15f);
                    }
                    else
                    {
                        Vector2 randomForce = new Vector2(Helpers.randomBetween(_rng, -1f, 1f), Helpers.randomBetween(_rng, -1f, 1f)) * 10f;

                        dronePhysicsComponent.bodies[0].ApplyForce(randomForce);
                    }
                }
            }
        }

        // Update
        public void update()
        {
            // Handle spawning battle drones
            handleSpawningBattleDrones(EntityManager.getEntitiesPossessing(ComponentType.BattleDroneOwner));

            // Follow owners
            handleFollowOwners(EntityManager.getEntitiesPossessing(ComponentType.BattleDroneOwner));
        }
    }
}
