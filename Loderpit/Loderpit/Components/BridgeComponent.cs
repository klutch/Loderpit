using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public class BridgeComponent : IComponent
    {
        private int _entityId;
        private List<Body> _bodies;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Bridge; } }
        public List<Body> bodies { get { return _bodies; } }

        public BridgeComponent(int entityId, List<Body> bodies)
        {
            _entityId = entityId;
            _bodies = bodies;
        }
    }
}
