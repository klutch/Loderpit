using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class AffectedBySpellEntitiesComponent : IComponent
    {
        private int _entityId;
        private List<int> _spellEntities;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.AffectedBySpellEntities; } }
        public List<int> spellEntities { get { return _spellEntities; } }

        public AffectedBySpellEntitiesComponent(int entityId)
        {
            _entityId = entityId;
            _spellEntities = new List<int>();
        }
    }
}
