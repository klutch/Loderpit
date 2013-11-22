using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class CombatTargetComponent : IComponent
    {
        private int _entityId;
        private int _targetId;

        public int entityId { get { return _entityId; } }
        public int targetId { get { return _targetId; } }
        public ComponentType componentType { get { return ComponentType.CombatTarget; } }

        public CombatTargetComponent(int entityId, int targetId)
        {
            _entityId = entityId;
            _targetId = targetId;
        }
    }
}
