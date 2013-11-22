using System;

namespace Loderpit.Components
{
    public class IgnoreBridgeRaycastComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.IgnoreBridgeRaycast; } }

        public IgnoreBridgeRaycastComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
