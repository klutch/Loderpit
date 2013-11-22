using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Formations;

namespace Loderpit.Components
{
    public class CreateRopeComponent : IComponent
    {
        private int _entityId;
        private Vector2 _position;
        private int _delay = 120;
        private LimitedRangeFormation _formationToRemove;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.CreateRope; } }
        public Vector2 position { get { return _position; } }
        public int delay { get { return _delay; } set { _delay = value; } }
        public LimitedRangeFormation formationToRemove { get { return _formationToRemove; } set { _formationToRemove = value; } }

        public CreateRopeComponent(int entityId, Vector2 position)
        {
            _entityId = entityId;
            _position = position;
        }
    }
}
