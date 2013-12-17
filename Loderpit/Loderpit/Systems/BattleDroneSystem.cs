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

        // Calculate drone position
        private Vector2 calculateDronePosition(int i)
        {
            float f = (float)Math.PI + ((float)i / 4f * (float)Math.PI);

            return new Vector2((float)Math.Cos(f), (float)Math.Sin(f)) * 2f;
        }

        // Handle follow owners
        private void handleFollowOwners(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                BattleDroneOwnerComponent battleDroneOwnerComponent = EntityManager.getBattleDroneOwnerComponent(entityId);
                PositionComponent ownerPositionComponent = EntityManager.getPositionComponent(entityId);

                for (int i = 0; i < battleDroneOwnerComponent.droneIds.Count; i++)
                {
                    int droneId = battleDroneOwnerComponent.droneIds[i];
                    Vector2 targetPosition = ownerPositionComponent.position + calculateDronePosition(i);
                    PhysicsComponent dronePhysicsComponent = EntityManager.getPhysicsComponent(droneId);
                    PositionComponent dronePositionComponent = EntityManager.getPositionComponent(droneId);
                    Vector2 relative = targetPosition - dronePositionComponent.position;
                    float length = relative.Length();

                    if (length > 3f)
                    {
                        dronePhysicsComponent.bodies[0].ApplyForce(relative * 25f);
                    }
                    else if (length > 1f)
                    {
                        dronePhysicsComponent.bodies[0].ApplyForce(relative * 15f);
                    }
                    else
                    {
                        Vector2 randomForce = new Vector2(Helpers.randomBetween(_rng, -1f, 1f), Helpers.randomBetween(_rng, -1f, 1f)) * 5f;

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
