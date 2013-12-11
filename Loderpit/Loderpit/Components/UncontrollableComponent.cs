using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class UncontrollableComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Uncontrollable; } }

        public UncontrollableComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
