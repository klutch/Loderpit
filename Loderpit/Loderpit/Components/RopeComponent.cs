using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public enum ClimbDirection
    {
        Up,
        Down
    }

    public class RopeComponent : IComponent
    {
        private int _entityId;
        private List<Body> _bodies;
        private Body _anchorBody;
        private ClimbDirection _climbDirection;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Rope; } }
        public List<Body> bodies { get { return _bodies; } }
        public ClimbDirection climbDirection { get { return _climbDirection; } }
        public Body anchorBody { get { return _anchorBody; } }

        public RopeComponent(int entityId, List<Body> bodies, Body anchorBody, ClimbDirection climbDirection)
        {
            _entityId = entityId;
            _bodies = bodies;
            _anchorBody = anchorBody;
            _climbDirection = climbDirection;
        }
    }
}
