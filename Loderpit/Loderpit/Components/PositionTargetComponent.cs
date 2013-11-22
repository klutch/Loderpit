using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public class PositionTargetComponent : IComponent
    {
        private int _entityId;
        private float _position;
        private float _tolerance;
        private Body _body;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.PositionTarget; } }
        public float position { get { return _body == null ? _position : _body.Position.X; } set { _position = value; } }
        public float tolerance { get { return _tolerance; } set { _tolerance = value; } }

        public PositionTargetComponent(int entityId, float position, float tolerance)
        {
            _entityId = entityId;
            _position = position;
            _tolerance = tolerance;
        }

        public PositionTargetComponent(int entityId, Body body, float tolerance)
        {
            _entityId = entityId;
            _body = body;
            _tolerance = tolerance;
        }
    }
}
