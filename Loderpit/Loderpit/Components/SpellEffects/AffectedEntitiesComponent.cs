using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class AffectedEntitiesComponent : IComponent
    {
        private int _entityId;
        private List<int> _entities;
        private List<Faction> _factionsToAffect;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.AffectedEntities; } }
        public List<int> entities { get { return _entities; } }
        public List<Faction> factionsToAffect { get { return _factionsToAffect; } set { _factionsToAffect = value; } }

        public AffectedEntitiesComponent(int entityId)
        {
            _entityId = entityId;
            _entities = new List<int>();
            _factionsToAffect = new List<Faction>();
        }
    }
}
