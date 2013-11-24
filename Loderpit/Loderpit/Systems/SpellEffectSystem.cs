using System;
using System.Collections.Generic;
using System.Diagnostics;
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

        // Rebuild the affected entities map
        private void rebuildAffectedEntities(List<int> entities)
        {
            _affectedEntitiesMap.Clear();

            foreach (int entityId in entities)
            {
                SpellEffectsComponent spellEffectsComponent = EntityManager.getSpellEffectsComponent(entityId);

                foreach (SpellEffect spellEffect in spellEffectsComponent.effects)
                {
                    // TODO: Handle rebuilding who's affected by different types of spell effects here
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
