using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // Find entities friendly to an entity within a certain range
        private List<int> findEntitiesWithinRange(int entityId, float radius, Faction factionToMatch)
        {
            PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
            List<int> entitiesWithFaction = EntityManager.getEntitiesPossessing(ComponentType.Faction);
            List<int> results = new List<int>();

            foreach (int targetEntityId in entitiesWithFaction)
            {
                PositionComponent targetPositionComponent;
                FactionComponent targetFactionComponent;
                Vector2 relative;

                // Skip self
                if (targetEntityId == entityId)
                {
                    continue;
                }

                targetPositionComponent = EntityManager.getPositionComponent(targetEntityId);
                targetFactionComponent = EntityManager.getFactionComponent(targetEntityId);
                relative = targetPositionComponent.position - positionComponent.position;

                // Check faction
                if (targetFactionComponent.faction != factionToMatch)
                {
                    continue;
                }

                // Check range
                if (relative.Length() <= radius)
                {
                    results.Add(targetEntityId);
                }
            }

            return results;
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
                SpellEffectsComponent spellEffectsComponent = EntityManager.getSpellEffectsComponent(entityId);
                FactionComponent factionComponent = EntityManager.getFactionComponent(entityId);
                List<int> entitiesToAdd = new List<int>();

                foreach (SpellEffect spellEffect in spellEffectsComponent.effects)
                {
                    // Add self to affected entity map
                    if (spellEffect.affectsSelf)
                    {
                        entitiesToAdd.Add(entityId);
                    }

                    // Add friendly entities to the affected entity map
                    if (spellEffect.affectsFriendly)
                    {
                        entitiesToAdd.AddRange(findEntitiesWithinRange(entityId, spellEffect.radius, factionComponent.faction));
                    }

                    // Add hostile entities to the affected entity map
                    if (spellEffect.affectsHostile)
                    {
                        entitiesToAdd.AddRange(findEntitiesWithinRange(entityId, spellEffect.radius, factionComponent.hostileFaction));
                    }

                    // Add neutral entities to the affected entity map
                    if (spellEffect.affectsNeutral)
                    {
                        entitiesToAdd.AddRange(findEntitiesWithinRange(entityId, spellEffect.radius, Faction.Neutral));
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
            SpellEffectsComponent spellEffectsComponent = EntityManager.getSpellEffectsComponent(entityId);

            Debug.Assert(!spellEffectsComponent.effects.Contains(spellEffect), String.Format("This entity already has a spell effect of type: {0}", spellEffect.type));

            spellEffectsComponent.effects.Add(spellEffect);
        }

        public void update()
        {
            List<int> spellEffectsEntities = EntityManager.getEntitiesPossessing(ComponentType.SpellEffects);

            // Rebuild the affected entities map
            rebuildAffectedEntities(spellEffectsEntities);
        }
    }
}
