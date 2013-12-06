using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class DamageShieldComponent : IComponent
    {
        private int _entityId;
        private string _damageDie;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DamageShield; } }
        public string damageDie { get { return _damageDie; } }

        public DamageShieldComponent(int entityId, string damageDie)
        {
            _entityId = entityId;
            _damageDie = damageDie;
        }
    }
}
