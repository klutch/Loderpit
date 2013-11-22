using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Loderpit.Formations;

namespace Loderpit.Components
{
    public class DestructibleObstacleComponent : IComponent
    {
        private int _entityId;
        private Body _body;
        private Dictionary<int, SplitFormation> _formationsToRemove;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DestructibleObstacle; } }
        public Body body { get { return _body; } }
        public Dictionary<int, SplitFormation> formationsToRemove { get { return _formationsToRemove; } }

        public DestructibleObstacleComponent(int entityId, Body body)
        {
            _entityId = entityId;
            _body = body;
            _formationsToRemove = new Dictionary<int, SplitFormation>();
        }
    }
}
