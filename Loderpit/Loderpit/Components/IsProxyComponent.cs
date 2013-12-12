using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class IsProxyComponent : IComponent
    {
        private int _entityId;
        private int _proxyForId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.IsProxy; } }
        public int proxyForId {get { return _proxyForId; }}

        public IsProxyComponent(int entityId, int proxyForId)
        {
            _entityId = entityId;
            _proxyForId = proxyForId;
        }
    }
}
