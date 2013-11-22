using System;
using System.Collections.Generic;
using Loderpit.Formations;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class GroupSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Group; } }

        public GroupSystem()
        {
        }

        // Get amount to advance formation positions by
        private float getAdvancementAmount(int speed)
        {
            int absSpeed = Math.Abs(speed);
            float value = 0f;

            switch (absSpeed)
            {
                case 1: value = 0.02f; break;
                case 2: value = 0.03f; break;
                case 3: value = 0.04f; break;
                default: value = 0f; break;
            }

            return speed < 0 ? -value : value;
        }

        // Find the active formation of an entity in a group
        public Formation getActiveFormationByEntityId(int entityId)
        {
            GroupComponent groupComponent = getGroupComponentContaining(entityId);

            if (groupComponent == null)
            {
                return null;
            }
            else
            {
                return groupComponent.activeFormation;
            }
        }

        // Get group component containing an entity
        public GroupComponent getGroupComponentContaining(int entityId)
        {
            List<int> groupEntities = EntityManager.getEntitiesPossessing(ComponentType.Group);

            foreach (int groupEntityId in groupEntities)
            {
                GroupComponent groupComponent = EntityManager.getGroupComponent(groupEntityId);

                foreach (int groupMemberEntityId in groupComponent.entities)
                {
                    if (groupMemberEntityId == entityId)
                    {
                        return groupComponent;
                    }
                }
            }

            return null;
        }

        // Increase speed
        public void increaseActiveFormationSpeed(int groupEntityId)
        {
            GroupComponent groupComponent = EntityManager.getGroupComponent(groupEntityId);
            Formation formation = groupComponent.activeFormation;
            int newSpeed = formation.speed + 1;

            if (newSpeed < formation.maxSpeed)
            {
                formation.speed = newSpeed;
                formation.storedSpeed = newSpeed;
            }
        }

        // Decrease speed
        public void decreaseActiveFormationSpeed(int groupEntityId)
        {
            GroupComponent groupComponent = EntityManager.getGroupComponent(groupEntityId);
            Formation formation = groupComponent.activeFormation;
            int newSpeed = formation.speed - 1;

            if (newSpeed > -formation.maxSpeed)
            {
                formation.speed = newSpeed;
                formation.storedSpeed = newSpeed;
            }
        }

        // Moves the formations forward based on the formation's movement speed
        public void advanceGroupFormations(List<int> groupEntities)
        {
            foreach (int entityId in groupEntities)
            {
                GroupComponent groupComponent = EntityManager.getGroupComponent(entityId);

                if (groupComponent != null)
                {
                    Formation formation = groupComponent.activeFormation;

                    formation.position += getAdvancementAmount(formation.speed);
                }
            }
        }

        // Handle limited range formations
        public void handleLimitedRangeFormations(List<int> groupEntities)
        {
            foreach (int groupEntityId in groupEntities)
            {
                GroupComponent groupComponent = EntityManager.getGroupComponent(groupEntityId);

                if (groupComponent != null && groupComponent.activeFormation.type == FormationType.LimitedRange)
                {
                    LimitedRangeFormation formation = (LimitedRangeFormation)groupComponent.activeFormation;

                    for (int slot = 0; slot < groupComponent.entities.Count; slot++)
                    {
                        float slotPosition = formation.getSlotPosition(slot);

                        if (slotPosition < formation.minPosition)
                        {
                            formation.position += slotPosition - formation.minPosition;
                            formation.speed = 0;
                            break;
                        }
                        else if (slotPosition > formation.maxPosition)
                        {
                            formation.position -= slotPosition - formation.maxPosition;
                            formation.speed = 0;
                            break;
                        }
                    }
                }
            }
        }

        public void update()
        {
            List<int> groupEntities = EntityManager.getEntitiesPossessing(ComponentType.Group);

            // Handle limited range formations
            handleLimitedRangeFormations(groupEntities);

            // Advance formation
            advanceGroupFormations(groupEntities);
        }
    }
}
