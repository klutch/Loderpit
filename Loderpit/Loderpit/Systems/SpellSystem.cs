using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Collision;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Components.SpellEffects;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class SpellSystem : ISystem
    {
        public SystemType systemType { get { return SystemType.Spell; } }

        public SpellSystem()
        {
        }

        // Clear affected entities
        private void clearAffectedEntities(List<int> affectedEntities, List<int> affectedBySpellEntities)
        {
            // Remove character entities from spell entities
            foreach (int entityId in affectedEntities)
            {
                AffectedEntitiesComponent affectedEntitiesComponent = EntityManager.getAffectedEntitiesComponent(entityId);
                AreaOfEffectComponent areaOfEffectComponent = EntityManager.getAreaOfEffectComponent(entityId);

                // Clear all affected entities, except for entities without an area of effect
                if (areaOfEffectComponent != null)
                {
                    affectedEntitiesComponent.entities.Clear();
                }
            }

            // Remove spell entities from character entities
            foreach (int entityId in affectedBySpellEntities)
            {
                AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(entityId);
                List<int> spellEntitiesToClear = new List<int>();

                foreach (int spellId in affectedBySpellEntitiesComponent.spellEntities)
                {
                    if (EntityManager.getAreaOfEffectComponent(spellId) != null)
                    {
                        spellEntitiesToClear.Add(spellId);
                    }
                }

                foreach (int spellId in spellEntitiesToClear)
                {
                    affectedBySpellEntitiesComponent.spellEntities.Remove(spellId);
                }
            }
        }

        // Handle time to live
        private void handleTimeToLive(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                TimeToLiveComponent timeToLiveComponent = EntityManager.getTimeToLiveComponent(entityId);

                if (timeToLiveComponent.delay <= 0)
                {
                    EntityManager.destroyEntity(entityId);
                }
                else
                {
                    timeToLiveComponent.delay--;
                }
            }
        }

        // Track entity positions
        private void trackEntityPositions(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                TrackEntityPositionComponent trackEntityPositionComponent = EntityManager.getTrackEntityPositionComponent(entityId);
                AreaOfEffectComponent areaOfEffectComponent = EntityManager.getAreaOfEffectComponent(entityId);
                PositionComponent positionComponent = EntityManager.getPositionComponent(trackEntityPositionComponent.targetEntityId);

                areaOfEffectComponent.sensor.Position = positionComponent.position;
            }
        }

        // Find affected entities
        private void findAffectedEntities(List<int> characterEntities)
        {
            foreach (int characterId in characterEntities)
            {
                PositionComponent positionComponent = EntityManager.getPositionComponent(characterId);
                List<Fixture> fixtures = SystemManager.physicsSystem.world.TestPointAll(positionComponent.position);
                AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(characterId);
                FactionComponent factionComponent = EntityManager.getFactionComponent(characterId);

                foreach (Fixture fixture in fixtures)
                {
                    int entityId;
                    AreaOfEffectComponent areaOfEffectComponent;
                    AffectedEntitiesComponent affectedEntitiesComponent;
                    bool acceptableFaction = false;

                    // Skip if no userdata
                    if (fixture.Body.UserData == null)
                    {
                        continue;
                    }

                    entityId = (int)fixture.Body.UserData;

                    // Skip if no area of effect component
                    if ((areaOfEffectComponent = EntityManager.getAreaOfEffectComponent(entityId)) == null)
                    {
                        continue;
                    }

                    affectedEntitiesComponent = EntityManager.getAffectedEntitiesComponent(entityId);

                    // Skip if faction is not acceptable
                    foreach (Faction faction in affectedEntitiesComponent.factionsToAffect)
                    {
                        if (factionComponent.faction == faction)
                        {
                            acceptableFaction = true;
                            break;
                        }
                    }
                    if (!acceptableFaction)
                    {
                        continue;
                    }

                    // Add entities to each other's affected component
                    affectedBySpellEntitiesComponent.spellEntities.Add(entityId);
                    affectedEntitiesComponent.entities.Add(characterId);
                }
            }
        }

        // Handle damage over time
        private void handleDamageOverTime(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                DamageOverTimeComponent damageOverTimeComponent = EntityManager.getDamageOverTimeComponent(entityId);

                if (damageOverTimeComponent.currentDelay == 0)
                {
                    AffectedEntitiesComponent affectedEntitiesComponent = EntityManager.getAffectedEntitiesComponent(entityId);
                    List<int> copyOfAffectedEntities = new List<int>(affectedEntitiesComponent.entities);   // the entities collection can be modified when an entity dies, so operate on a copy of it

                    damageOverTimeComponent.currentDelay = damageOverTimeComponent.baseDelay;

                    foreach (int affectedId in copyOfAffectedEntities)
                    {
                        if (EntityManager.doesEntityExist(affectedId))
                        {
                            SystemManager.combatSystem.applySpellDamage(affectedId, Roller.roll(damageOverTimeComponent.damageDie));
                        }
                    }
                }
                else
                {
                    damageOverTimeComponent.currentDelay--;
                }
            }
        }

        // Update
        public void update()
        {
            // Clear affected entities
            clearAffectedEntities(EntityManager.getEntitiesPossessing(ComponentType.AffectedEntities), EntityManager.getEntitiesPossessing(ComponentType.AffectedBySpellEntities));

            // Handle time to live
            handleTimeToLive(EntityManager.getEntitiesPossessing(ComponentType.TimeToLive));

            // Track positions
            trackEntityPositions(EntityManager.getEntitiesPossessing(ComponentType.TrackEntityPosition));

            // Find affected entities
            findAffectedEntities(EntityManager.getEntitiesPossessing(ComponentType.Character));

            // Handle damage over time
            handleDamageOverTime(EntityManager.getEntitiesPossessing(ComponentType.DamageOverTime));
        }
    }
}
