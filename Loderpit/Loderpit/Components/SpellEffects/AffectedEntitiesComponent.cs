using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class AffectedEntitiesComponent : IComponent
    {
        private int _entityId;
        private List<int> _entities;
        private List<Faction> _factionsToAccept;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.AffectedEntities; } }
        public List<int> entities { get { return _entities; } }
        public List<Faction> factionsToAccept { get { return _factionsToAccept; } }

        public AffectedEntitiesComponent(int entityId)
        {
            _entityId = entityId;
            _entities = new List<int>();
            _factionsToAccept = new List<Faction>();
        }
    }
}
