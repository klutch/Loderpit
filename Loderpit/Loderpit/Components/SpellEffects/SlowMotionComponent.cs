using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class SlowMotionComponent : IComponent
    {
        private int _entityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.SlowMotion; } }

        public SlowMotionComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
