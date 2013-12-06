using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class TimeToLiveComponent : IComponent
    {
        private int _entityId;
        private int _delay;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.TimeToLive; } }

        public TimeToLiveComponent(int entityId, int delay)
        {
            _entityId = entityId;
            _delay = delay;
        }
    }
}
