using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public class PositionComponent : IComponent
    {
        private int _entityId;
        private Vector2 _position;
        private Body _body;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Position; } }
        public Vector2 position { get { return _body == null ? _position : _body.Position; } }

        public PositionComponent(int entityId, Vector2 position)
        {
            _position = position;
        }

        public PositionComponent(int entityId, Body body)
        {
            _body = body;
        }
    }
}
