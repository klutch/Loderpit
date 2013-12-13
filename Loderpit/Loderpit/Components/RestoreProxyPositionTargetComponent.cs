using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class RestoreProxyPositionTargetComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.RestoreProxyPositionTarget; } }

        public RestoreProxyPositionTargetComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
