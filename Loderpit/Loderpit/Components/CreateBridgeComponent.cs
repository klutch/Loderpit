using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Formations;

namespace Loderpit.Components
{
    public class CreateBridgeComponent : IComponent
    {
        private int _entityId;
        private Vector2 _positionA;
        private Vector2 _positionB;
        private int _delay = 120;
        private LimitedRangeFormation _formationToRemove;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.CreateBridge; } }
        public Vector2 positionA { get { return _positionA; } }
        public Vector2 positionB { get { return _positionB; } }
        public int delay { get { return _delay; } set { _delay = value; } }
        public LimitedRangeFormation formationToRemove { get { return _formationToRemove; } set { _formationToRemove = value; } }

        public CreateBridgeComponent(int entityId, Vector2 positionA, Vector2 positionB)
        {
            _entityId = entityId;
            _positionA = positionA;
            _positionB = positionB;
        }
    }
}
