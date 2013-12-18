using System;
using System.Collections.Generic;
using System.Diagnostics;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Components.SpellEffects;
using Loderpit.Formations;

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
        public static void destroyEntity(int idToDestroy)
        {
            GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(idToDestroy);
            DestructibleObstacleComponent destructibleObstacleComponent = EntityManager.getDestructibleObstacleComponent(idToDestroy);
            AreaOfEffectComponent areaOfEffectComponent = EntityManager.getAreaOfEffectComponent(idToDestroy);
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(idToDestroy);
            AffectedEntitiesComponent affectedEntitiesComponent = EntityManager.getAffectedEntitiesComponent(idToDestroy);
            PhysicsComponent physicsComponent = EntityManager.getPhysicsComponent(idToDestroy);
            IsProxyComponent isProxyComponent = EntityManager.getIsProxyComponent(idToDestroy);

            // Handle removal from a group
            if (groupComponent != null)
            {
                groupComponent.entities.Remove(idToDestroy);
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

            // Destroy area of effect body if it exists
            if (areaOfEffectComponent != null)
            {
                SystemManager.physicsSystem.world.RemoveBody(areaOfEffectComponent.sensor);
            }

            // Handle spell entity references (assuming this is a character being affected by spells)
            if (affectedBySpellEntitiesComponent != null)
            {
                foreach (int spellId in affectedBySpellEntitiesComponent.spellEntities)
                {
                    AffectedEntitiesComponent spellsAffectedEntitiesComponent;

                    // Skip if spell is dead
                    if (!EntityManager.doesEntityExist(spellId))
                    {
                        continue;
                    }

                    spellsAffectedEntitiesComponent = EntityManager.getAffectedEntitiesComponent(spellId);

                    // Remove (character) entity being destroyed from the spell entity's list of affected entities
                    spellsAffectedEntitiesComponent.entities.Remove(idToDestroy);
                }
            }

            // Handle spell entity references (assuming this is a spell affecting a character)
            if (affectedEntitiesComponent != null)
            {
                foreach (int affectedId in affectedEntitiesComponent.entities)
                {
                    AffectedBySpellEntitiesComponent charactersAffectedBySpellEntitiesComponent;

                    // Skip if entity is dead
                    if (!EntityManager.doesEntityExist(affectedId))
                    {
                        continue;
                    }

                    charactersAffectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(affectedId);

                    // Remove (spell) entity being destroyed from a character's list of spell entities affecting it
                    charactersAffectedBySpellEntitiesComponent.spellEntities.Remove(idToDestroy);
                }
            }

            // Handle physics component
            if (physicsComponent != null)
            {
                foreach (Body body in physicsComponent.bodies)
                {
                    SystemManager.physicsSystem.world.RemoveBody(body);
                }
            }

            // Handle proxy reference
            if (isProxyComponent != null)
            {
                EntityManager.removeComponent(isProxyComponent.proxyForId, ComponentType.HasProxy);
            }

            // Finally, remove the entity from the dictionary
            _entities.Remove(idToDestroy);
        }

        // Destroy all entities
        public static void destroyAllEntities()
        {
            foreach (int entityId in _entities.Keys)
            {
                destroyEntity(entityId);
            }
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
        public static PerformingSkillsComponent getPerformingSkillsComponent(int entityId) { return getComponent<PerformingSkillsComponent>(entityId, ComponentType.PerformingSkills); }
        public static ShieldComponent getShieldComponent(int entityId) { return getComponent<ShieldComponent>(entityId, ComponentType.Shield); }
        public static ExternalMovementSpeedsComponent getExternalMovementSpeedsComponent(int entityId) { return getComponent<ExternalMovementSpeedsComponent>(entityId, ComponentType.ExternalMovementSpeeds); }
        public static AffectedEntitiesComponent getAffectedEntitiesComponent(int entityId) { return getComponent<AffectedEntitiesComponent>(entityId, ComponentType.AffectedEntities); }
        public static AreaOfEffectComponent getAreaOfEffectComponent(int entityId) { return getComponent<AreaOfEffectComponent>(entityId, ComponentType.AreaOfEffect); }
        public static DamageOverTimeComponent getDamageOverTimeComponent(int entityId) { return getComponent<DamageOverTimeComponent>(entityId, ComponentType.DamageOverTime); }
        public static DamageShieldComponent getDamageShieldComponent(int entityId) { return getComponent<DamageShieldComponent>(entityId, ComponentType.DamageShield); }
        public static StatModifierComponent getStatModifierComponent(int entityId) { return getComponent<StatModifierComponent>(entityId, ComponentType.StatModifier); }
        public static TimeToLiveComponent getTimeToLiveComponent(int entityId) { return getComponent<TimeToLiveComponent>(entityId, ComponentType.TimeToLive); }
        public static TrackEntityPositionComponent getTrackEntityPositionComponent(int entityId) { return getComponent<TrackEntityPositionComponent>(entityId, ComponentType.TrackEntityPosition); }
        public static AffectedBySpellEntitiesComponent getAffectedBySpellEntitiesComponent(int entityId) { return getComponent<AffectedBySpellEntitiesComponent>(entityId, ComponentType.AffectedBySpellEntities); }
        public static ProcComponent getProcComponent(int entityId) { return getComponent<ProcComponent>(entityId, ComponentType.Proc); }
        public static TimedExplosionComponent getTimedExplosionComponent(int entityId) { return getComponent<TimedExplosionComponent>(entityId, ComponentType.TimedExplosion); }
        public static PhysicsComponent getPhysicsComponent(int entityId) { return getComponent<PhysicsComponent>(entityId, ComponentType.Physics); }
        public static DispellableComponent getDispellableComponent(int entityId) { return getComponent<DispellableComponent>(entityId, ComponentType.Dispellable); }
        public static DamageTransferComponent getDamageTransferComponent(int entityId) { return getComponent<DamageTransferComponent>(entityId, ComponentType.DamageTransfer); }
        public static RiposteComponent getRiposteComponent(int entityId) { return getComponent<RiposteComponent>(entityId, ComponentType.Riposte); }
        public static SpellTypeComponent getSpellTypeComponent(int entityId) { return getComponent<SpellTypeComponent>(entityId, ComponentType.SpellType); }
        public static DamageMitigationComponent getDamageMitigationComponent(int entityId) { return getComponent<DamageMitigationComponent>(entityId, ComponentType.DamageMitigation); }
        public static BasicCombatAIComponent getBasicCombatAiComponent(int entityId) { return getComponent<BasicCombatAIComponent>(entityId, ComponentType.BasicCombatAI); }
        public static FrenzyAIComponent getFrenzyAiComponent(int entityId) { return getComponent<FrenzyAIComponent>(entityId, ComponentType.FrenzyAI); }
        public static DotDamageModifierComponent getDotDamageModifierComponent(int entityId) { return getComponent<DotDamageModifierComponent>(entityId, ComponentType.DotDamageModifier); }
        public static ExternalForceComponent getExternalForceComponent(int entityId) { return getComponent<ExternalForceComponent>(entityId, ComponentType.ExternalForce); }
        public static HealOverTimeComponent getHealOverTimeComponent(int entityId) { return getComponent<HealOverTimeComponent>(entityId, ComponentType.HealOverTime); }
        public static SpellOwnerComponent getSpellOwnerComponent(int entityId) { return getComponent<SpellOwnerComponent>(entityId, ComponentType.SpellOwner); }
        public static HasProxyComponent getHasProxyComponent(int entityId) { return getComponent<HasProxyComponent>(entityId, ComponentType.HasProxy); }
        public static IsProxyComponent getIsProxyComponent(int entityId) { return getComponent<IsProxyComponent>(entityId, ComponentType.IsProxy); }
        public static BattleDroneOwnerComponent getBattleDroneOwnerComponent(int entityId) { return getComponent<BattleDroneOwnerComponent>(entityId, ComponentType.BattleDroneOwner); }
        public static BloodColorComponent getBloodColorComponent(int entityId) { return getComponent<BloodColorComponent>(entityId, ComponentType.BloodColor); }
    }
}
