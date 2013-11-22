using System;

namespace Loderpit.Components
{
    public class IgnoreRopeRaycastComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.IgnoreRopeRaycast; } }

        public IgnoreRopeRaycastComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
