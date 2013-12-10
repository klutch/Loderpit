using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class AIComponent : IComponent
    {
        private int _entityId;
        private AIType _aiType;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.AI; } }
        public AIType aiType { get { return _aiType; } }

        public AIComponent(int entityId, AIType aiType)
        {
            _entityId = entityId;
            _aiType = aiType;
        }
    }
}
