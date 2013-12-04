using System;
using System.Collections.Generic;
using System.Diagnostics;
using FarseerPhysics.Collision;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Skills;
using Loderpit.SpellEffects;

namespace Loderpit.Systems
{
    public class SpellEffectSystem : ISystem
    {
        private Dictionary<int, List<SpellEffect>> _affectedEntitiesMap;

        public SystemType systemType { get { return SystemType.SpellEffect; } }

        public SpellEffectSystem()
        {
            _affectedEntitiesMap = new Dictionary<int, List<SpellEffect>>();
        }

        // Get spell effects affecting a certain entity
        public List<SpellEffect> getSpellEffectsAffecting(int entityId)
        {
            if (_affectedEntitiesMap.ContainsKey(entityId))
            {
                return _affectedEntitiesMap[entityId];
            }
            else
            {
                return new List<SpellEffect>();
            }
        }

        // Add an affected entity to the affected entity map
        private void addAffectedEntity(int entityId, SpellEffect spellEffect)
        {
            if (!_affectedEntitiesMap.ContainsKey(entityId))
            {
                _affectedEntitiesMap.Add(entityId, new List<SpellEffect>());
            }

            _affectedEntitiesMap[entityId].Add(spellEffect);
        }

        // Rebuild the affected entities map
        private void rebuildAffectedEntities(List<int> entities)
        {
            _affectedEntitiesMap.Clear();

            foreach (int entityId in entities)
            {
                ActiveSpellEffectsComponent spellEffectsComponent = EntityManager.getSpellEffectsComponent(entityId);
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                List<int> entitiesToAdd = new List<int>();

                foreach (SpellEffect spellEffect in spellEffectsComponent.effects)
                {
                    // Add self to affected entity map
                    if (spellEffect.affectsSelf)
                    {
                        entitiesToAdd.Add(entityId);
                    }

                    // AoE/aura spell effects
                    if (spellEffect is IAoESpellEffect)
                    {
                        IAoESpellEffect aeSpellEffect = spellEffect as IAoESpellEffect;

                        // Add friendly entities to the affected entity map
                        if (spellEffect.affectsFriendly)
                        {
                            entitiesToAdd.AddRange(Helpers.findEntitiesWithinRange(entityId, aeSpellEffect.radius, factionComponent.faction));
                        }

                        // Add hostile entities to the affected entity map
                        if (spellEffect.affectsHostile)
                        {
                            entitiesToAdd.AddRange(Helpers.findEntitiesWithinRange(entityId, aeSpellEffect.radius, factionComponent.hostileFaction));
                        }

                        // Add neutral entities to the affected entity map
                        if (spellEffect.affectsNeutral)
                        {
                            entitiesToAdd.AddRange(Helpers.findEntitiesWithinRange(entityId, aeSpellEffect.radius, Faction.Neutral));
                        }
                    }

                    // Add accumulated entities to the affected entity map
                    foreach (int affectedEntityId in entitiesToAdd)
                    {
                        addAffectedEntity(affectedEntityId, spellEffect);
                    }
                }
            }
        }

        // Apply all skills' passive spell effects once at the beginning of every level
        public void applyAllSkillPassiveSpellEffects()
        {
            List<int> skillsEntities = EntityManager.getEntitiesPossessing(ComponentType.Skills);

            foreach (int entityId in skillsEntities)
            {
                SkillsComponent skillsComponent = EntityManager.getSkillsComponent(entityId);

                foreach (Skill skill in skillsComponent.skills)
                {
                    foreach (SpellEffect spellEffect in skill.passiveSpellEffects)
                    {
                        applySpellEffect(entityId, spellEffect);
                    }
                }
            }
        }

        // Apply a spell effect to an entity
        public void applySpellEffect(int entityId, SpellEffect spellEffect)
        {
            ActiveSpellEffectsComponent spellEffectsComponent = EntityManager.getSpellEffectsComponent(entityId);

            Debug.Assert(!spellEffectsComponent.effects.Contains(spellEffect), String.Format("This entity already has a spell effect of type: {0}", spellEffect.type));

            spellEffectsComponent.effects.Add(spellEffect);
        }

        // Remove a spell effect from an entity
        public void removeSpellEffect(int entityId, SpellEffect spellEffect)
        {
            ActiveSpellEffectsComponent spellEffectsComponent = EntityManager.getSpellEffectsComponent(entityId);

            Debug.Assert(spellEffectsComponent.effects.Contains(spellEffect), String.Format("This entity doesn't have the spell effect it's trying to remove: {0}", spellEffect.type));

            spellEffectsComponent.effects.Remove(spellEffect);
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

        // Handle spell effect time to level
        private void handleTTL()
        {
            foreach (KeyValuePair<int, List<SpellEffect>> entitySpellsPair in _affectedEntitiesMap)
            {
                int entityId = entitySpellsPair.Key;
                List<SpellEffect> spellEffects = entitySpellsPair.Value;

                foreach (SpellEffect spellEffect in spellEffects)
                {
                    if (spellEffect.timeToLive > 0)
                    {
                        spellEffect.timeToLive--;
                    }

                    if (spellEffect.timeToLive == 0)
                    {
                        removeSpellEffect(entityId, spellEffect);
                    }
                }
            }
        }

        // Handle DoT spell effects
        private void handleDoTSpellEffects()
        {
            foreach (KeyValuePair<int, List<SpellEffect>> entitySpellsPair in _affectedEntitiesMap)
            {
                int entityId = entitySpellsPair.Key;
                List<SpellEffect> spellEffects = entitySpellsPair.Value;

                foreach (SpellEffect spellEffect in spellEffects)
                {
                    if (spellEffect is IDoTSpellEffect)
                    {
                        IDoTSpellEffect dotSpellEffect = spellEffect as IDoTSpellEffect;

                        if (dotSpellEffect.currentDelay > 0)
                        {
                            dotSpellEffect.currentDelay--;
                        }

                        if (dotSpellEffect.currentDelay == 0)
                        {
                            if (EntityManager.doesEntityExist(entityId))
                            {
                                dotSpellEffect.onTick(spellEffect.sourceEntityId, entityId);
                            }
                            dotSpellEffect.currentDelay = dotSpellEffect.baseDelay;
                        }
                    }
                }
            }
        }

        // Update
        public void update()
        {
            List<int> spellEffectsEntities = EntityManager.getEntitiesPossessing(ComponentType.ActiveSpellEffects);

            // Rebuild the affected entities map
            rebuildAffectedEntities(spellEffectsEntities);

            // Handle DoT spell effects
            handleDoTSpellEffects();

            // Handle spell effect's time to live
            handleTTL();
        }
    }
}
