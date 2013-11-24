using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Components
{
    public class SpellEffectsComponent : IComponent
    {
        private int _entityId;
        private List<SpellEffect> _effects;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.SpellEffects; } }
        public List<SpellEffect> effects { get { return _effects; } }

        public SpellEffectsComponent(int entityId)
        {
            _entityId = entityId;
            _effects = new List<SpellEffect>();
        }
    }
}
