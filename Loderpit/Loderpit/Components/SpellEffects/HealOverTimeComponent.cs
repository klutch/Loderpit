using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class HealOverTimeComponent : IComponent
    {
        private int _entityId;
        private string _healDie;
        private int _currentDelay;
        private int _baseDelay;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.HealOverTime; } }
        public string healDie { get { return _healDie; } }
        public int currentDelay { get { return _currentDelay; } set { _currentDelay = value; } }
        public int baseDelay { get { return _baseDelay; } }

        public HealOverTimeComponent(int entityId, string healDie, int baseDelay)
        {
            _entityId = entityId;
            _healDie = healDie;
            _baseDelay = baseDelay;
        }
    }
}
