using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class FrenzyAIComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.FrenzyAI; } }

        public FrenzyAIComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
