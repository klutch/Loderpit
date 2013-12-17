using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class BattleDroneOwnerComponent : IComponent
    {
        private int _entityId;
        private List<int> _droneIds;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.BattleDroneOwner; } }
        public List<int> droneIds { get { return _droneIds; } }

        public BattleDroneOwnerComponent(int entityId)
        {
            _entityId = entityId;
            _droneIds = new List<int>();
        }
    }
}
