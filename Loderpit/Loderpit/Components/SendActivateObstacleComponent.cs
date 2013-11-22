using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class SendActivateObstacleComponent : IComponent
    {
        private int _entityId;
        private List<int> _receivingEntities;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.SendActivateObstacle; } }
        public List<int> receivingEntities { get { return _receivingEntities; } }

        public SendActivateObstacleComponent(int entityId, List<int> receivingEntities)
        {
            _entityId = entityId;
            _receivingEntities = receivingEntities;
        }
    }
}
