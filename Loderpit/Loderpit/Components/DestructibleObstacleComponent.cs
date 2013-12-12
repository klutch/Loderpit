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
        private List<Faction> _factionsToBlock;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DestructibleObstacle; } }
        public Body body { get { return _body; } }
        public Dictionary<int, SplitFormation> formationsToRemove { get { return _formationsToRemove; } }
        public List<Faction> factionsToBlock { get { return _factionsToBlock; } }

        public DestructibleObstacleComponent(int entityId, Body body, List<Faction> factionsToBlock)
        {
            _entityId = entityId;
            _body = body;
            _formationsToRemove = new Dictionary<int, SplitFormation>();
            _factionsToBlock = factionsToBlock;
        }
    }
}
