using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class HasProxyComponent : IComponent
    {
        private int _entityId;
        private int _proxyId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.HasProxy; } }
        public int proxyId { get { return _proxyId; } }

        public HasProxyComponent(int entityId, int proxyId)
        {
            _entityId = entityId;
            _proxyId = proxyId;
        }
    }
}
