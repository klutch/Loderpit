using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class DamageOverTimeComponent : IComponent
    {
        private int _entityId;
        private string _damageDie;
        private int _baseDelay;
        private int _currentDelay;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DamageOverTime; } }
        public string damageDie { get { return _damageDie; } }
        public int baseDelay { get { return _baseDelay; } }
        public int currentDelay { get { return _currentDelay; } set { _currentDelay = value; } }

        public DamageOverTimeComponent(int entityId, string damageDie, int baseDelay)
        {
            _entityId = entityId;
            _damageDie = damageDie;
            _baseDelay = baseDelay;
            _currentDelay = baseDelay;
        }
    }
}
