using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class SpellTypeComponent : IComponent
    {
        private int _entityId;
        private SpellType _spellType;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.SpellType; } }
        public SpellType spellType { get { return _spellType; } }

        public SpellTypeComponent(int entityId, SpellType spellType)
        {
            _entityId = entityId;
            _spellType = spellType;
        }
    }
}
