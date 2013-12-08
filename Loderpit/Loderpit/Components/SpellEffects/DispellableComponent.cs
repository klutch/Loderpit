using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class DispellableComponent : IComponent
    {
        private int _entityId;
        private List<Faction> _dispellableByFactions;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Dispellable; } }
        public List<Faction> dispellableByFactions { get { return _dispellableByFactions; } }

        public DispellableComponent(int entityId, List<Faction> dispellableByFactions)
        {
            _entityId = entityId;
            _dispellableByFactions = dispellableByFactions;
        }
    }
}
