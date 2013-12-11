using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public class PhysicsComponent : IComponent
    {
        private int _entityId;
        private List<Body> _bodies;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Physics; } }
        public List<Body> bodies { get { return _bodies; } }

        public PhysicsComponent(int entityId, List<Body> bodies = null)
        {
            _entityId = entityId;
            _bodies = bodies ?? new List<Body>();
        }
    }
}
