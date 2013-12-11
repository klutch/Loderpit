using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class BasicCombatAIComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.BasicCombatAI; } }

        public BasicCombatAIComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
