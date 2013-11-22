using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public class ReceiveActivateObstacleFallComponent : IComponent
    {
        private int _entityId;
        private Body _body;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.ReceiveActivateObstacleFall; } }
        public Body body { get { return _body; } }

        public ReceiveActivateObstacleFallComponent(int entityId, Body body)
        {
            _entityId = entityId;
            _body = body;
        }
    }
}
