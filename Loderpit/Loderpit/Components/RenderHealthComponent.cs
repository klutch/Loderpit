using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class RenderHealthComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.RenderHealth; } }

        public RenderHealthComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
