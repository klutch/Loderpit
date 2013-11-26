﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Loderpit.Components;

namespace Loderpit.Managers
{
    public class EntityManager
    {
        private static Dictionary<int, Dictionary<ComponentType, IComponent>> _entities = new Dictionary<int, Dictionary<ComponentType, IComponent>>();

        // Get the first unused entity id
        private static int getUnusedId()
        {
            int current = 0;

            while (_entities.ContainsKey(current))
            {
                current++;
            }

            return current;
        }

        // Create an entity
        public static int createEntity()
        {
            int id = getUnusedId();

            _entities.Add(id, new Dictionary<ComponentType, IComponent>());

            return id;
        }

        // Check to see if an entity exists
        public static bool doesEntityExist(int id)
        {
            return _entities.ContainsKey(id);
        }

        // Add a component to and entity
        public static void addComponent(int entityId, IComponent component)
        {
            Debug.Assert(!_entities[entityId].ContainsKey(component.componentType));

            _entities[entityId].Add(component.componentType, component);
        }

        // Remove a component from an entity
        public static void removeComponent(int entityId, ComponentType componentType)
        {
            if (_entities[entityId].ContainsKey(componentType))
            {
                _entities[entityId].Remove(componentType);
            }
        }

        // Get a list of entities that possess a certain component
        public static List<int> getEntitiesPossessing(ComponentType componentType)
        {
            List<int> entities = new List<int>();

            foreach (KeyValuePair<int, Dictionary<ComponentType, IComponent>> entityPair in _entities)
            {
                if (entityPair.Value.ContainsKey(componentType))
                {
                    entities.Add(entityPair.Key);
                }
            }

            return entities;
        }

        // Get a component
        private static T getComponent<T>(int entityId, ComponentType componentType)
        {
            IComponent component = null;

            if (_entities.ContainsKey(entityId))
            {
                _entities[entityId].TryGetValue(componentType, out component);
            }
            else
            {
                // TODO: Throw an assert right here, because this should ideally never happen...
            }

            return (T)component;
        }

        // Destroy an entity
        public static void destroyEntity(int entityId)
        {
            _entities.Remove(entityId);
        }

        // Destroy all entities
        public static void destroyAllEntities()
        {
            _entities.Clear();
        }

        // Helper methods
        public static BridgeComponent getBridgeComponent(int entityId) { return getComponent<BridgeComponent>(entityId, ComponentType.Bridge); }
        public static CharacterComponent getCharacterComponent(int entityId) { return getComponent<CharacterComponent>(entityId, ComponentType.Character); }
        public static DestructibleObstacleComponent getDestructibleObstacleComponent(int entityId) { return getComponent<DestructibleObstacleComponent>(entityId, ComponentType.DestructibleObstacle); }
        public static GroundBodyComponent getGroundBodyComponent(int entityId) { return getComponent<GroundBodyComponent>(entityId, ComponentType.GroundBody); }
        public static IgnoreBridgeRaycastComponent getIgnoreBridgeRaycastComponent(int entityId) { return getComponent<IgnoreBridgeRaycastComponent>(entityId, ComponentType.IgnoreBridgeRaycast); }
        public static IgnoreRopeRaycastComponent getIgnoreRopeRaycastComponent(int entityId) { return getComponent<IgnoreRopeRaycastComponent>(entityId, ComponentType.IgnoreRopeRaycast); }
        public static RopeComponent getRopeComponent(int entityId) { return getComponent<RopeComponent>(entityId, ComponentType.Rope); }
        public static RopeGrabComponent getRopeGrabComponent(int entityId) { return getComponent<RopeGrabComponent>(entityId, ComponentType.RopeGrab); }
        public static RopeGrabExclusionComponent getRopeGrabExclusionComponent(int entityId) { return getComponent<RopeGrabExclusionComponent>(entityId, ComponentType.RopeGrabExclusion); }
        public static PositionTargetComponent getPositionTargetComponent(int entityId) { return getComponent<PositionTargetComponent>(entityId, ComponentType.PositionTarget); }
        public static StatsComponent getStatsComponent(int entityId) { return getComponent<StatsComponent>(entityId, ComponentType.Stats); }
        public static PositionComponent getPositionComponent(int entityId) { return getComponent<PositionComponent>(entityId, ComponentType.Position); }
        public static CeilingComponent getCeilingComponent(int entityId) { return getComponent<CeilingComponent>(entityId, ComponentType.Ceiling); }
        public static SendActivateObstacleComponent getSendActivateObstacleComponent(int entityId) { return getComponent<SendActivateObstacleComponent>(entityId, ComponentType.SendActivateObstacle); }
        public static ReceiveActivateObstacleFallComponent getReceiveActivateObstacleFallComponent(int entityId) { return getComponent<ReceiveActivateObstacleFallComponent>(entityId, ComponentType.ReceiveActivateObstacleFall); }
        public static GroupComponent getGroupComponent(int entityId) { return getComponent<GroupComponent>(entityId, ComponentType.Group); }
        public static SkillsComponent getSkillsComponent(int entityId) { return getComponent<SkillsComponent>(entityId, ComponentType.Skills); }
        public static FactionComponent getFactionComponent(int entityId) { return getComponent<FactionComponent>(entityId, ComponentType.Faction); }
        public static CombatTargetComponent getCombatTargetComponent(int entityId) { return getComponent<CombatTargetComponent>(entityId, ComponentType.CombatTarget); }
        public static IncapacitatedComponent getIncapacitatedComponent(int entityId) { return getComponent<IncapacitatedComponent>(entityId, ComponentType.Incapacitated); }
        public static IsTouchingEndLevelComponent getIsTouchingEndLevelComponent(int entityId) { return getComponent<IsTouchingEndLevelComponent>(entityId, ComponentType.IsTouchingEndLevel); }
        public static ActiveSpellEffectsComponent getSpellEffectsComponent(int entityId) { return getComponent<ActiveSpellEffectsComponent>(entityId, ComponentType.ActiveSpellEffects); }
        public static PerformingSkillsComponent getPerformingSkillsComponent(int entityId) { return getComponent<PerformingSkillsComponent>(entityId, ComponentType.PerformingSkills); }
    }
}
