using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Components
{
    public class ActiveSpellEffectsComponent : IComponent
    {
        private int _entityId;
        private List<SpellEffect> _effects;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.ActiveSpellEffects; } }
        public List<SpellEffect> effects { get { return _effects; } }

        public ActiveSpellEffectsComponent(int entityId)
        {
            _entityId = entityId;
            _effects = new List<SpellEffect>();
        }
    }
}
