using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class IncapacitatedComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Incapacitated; } }

        public IncapacitatedComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
