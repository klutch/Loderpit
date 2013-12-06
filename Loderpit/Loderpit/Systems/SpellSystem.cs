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

        // Create explosion
        public List<int> createExplosion(int entityId, Vector2 position, float radius, int damage, float force)
        {
            AABB aabb = new AABB(position - new Vector2(radius, radius), position + new Vector2(radius, radius));
            List<int> affectedEntities = new List<int>();

            // Handle bodies with physical representations
            SystemManager.physicsSystem.world.QueryAABB((fixture) =>
            {
                int targetEntityId;
                Vector2 relative = fixture.Body.Position - position;
                Vector2 normal = Vector2.Normalize(relative);
                CharacterComponent characterComponent;

                // Skip fixtures whose bodies don't have a user data
                if (fixture.Body.UserData == null)
                {
                    return true;
                }

                targetEntityId = (int)fixture.Body.UserData;

                // Skip if outside of radius
                if (relative.Length() >= radius)
                {
                    return true;
                }

                // Store as an affected entity if we haven't already
                if (affectedEntities.Contains(targetEntityId))
                {
                    return true;
                }

                // Skip entities that aren't characters
                // TODO: handle other types of entities
                if ((characterComponent = EntityManager.getCharacterComponent(targetEntityId)) == null)
                {
                    return true;
                }

                characterComponent.body.ApplyLinearImpulse(new Vector2(0, -1f));
                characterComponent.body.ApplyForce(normal * force);
                SystemManager.combatSystem.applySpellDamage(targetEntityId, damage);
                affectedEntities.Add(targetEntityId);

                return true;
            },
                ref aabb);

            // TODO: Handle any other types of entities here...

            return affectedEntities;
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
                    AreaOfEffectComponent areaOfEffectComponent = EntityManager.getAreaOfEffectComponent(entityId);

                    // Destroy area of effect body if it exists
                    if (areaOfEffectComponent != null)
                    {
                        SystemManager.physicsSystem.world.RemoveBody(areaOfEffectComponent.sensor);
                    }

                    // Destroy entity
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

                    damageOverTimeComponent.currentDelay = damageOverTimeComponent.baseDelay;

                    foreach (int affectedId in affectedEntitiesComponent.entities)
                    {
                        SystemManager.combatSystem.applySpellDamage(affectedId, Roller.roll(damageOverTimeComponent.damageDie));
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
            List<int> affectedEntities = EntityManager.getEntitiesPossessing(ComponentType.AffectedEntities);
            List<int> affectedBySpellEntities = EntityManager.getEntitiesPossessing(ComponentType.AffectedBySpellEntities);
            List<int> characterEntities = EntityManager.getEntitiesPossessing(ComponentType.Character);
            List<int> trackPositionEntities = EntityManager.getEntitiesPossessing(ComponentType.TrackEntityPosition);
            List<int> timeToLiveEntities = EntityManager.getEntitiesPossessing(ComponentType.TimeToLive);
            List<int> damageOverTimeEntities = EntityManager.getEntitiesPossessing(ComponentType.DamageOverTime);

            // Clear affected entities
            clearAffectedEntities(affectedEntities, affectedBySpellEntities);

            // Handle time to live
            handleTimeToLive(timeToLiveEntities);

            // Track positions
            trackEntityPositions(trackPositionEntities);

            // Find affected entities
            findAffectedEntities(characterEntities);

            // Handle damage over time
            handleDamageOverTime(damageOverTimeEntities);
        }
    }
}
