using System;
using System.Collections.Generic;
using Loderpit.Components;
using Loderpit.Managers;
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
                    // TODO: Handle different types of spell effects here
                }
            }
        }

        public void update()
        {
            List<int> spellEffectsEntities = EntityManager.getEntitiesPossessing(ComponentType.SpellEffects);

            // Rebuild the affected entities map
            rebuildAffectedEntities(spellEffectsEntities);
        }
    }
}
