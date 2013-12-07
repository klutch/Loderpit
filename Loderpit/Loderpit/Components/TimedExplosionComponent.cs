using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class TimedExplosionComponent : IComponent
    {
        private int _entityId;
        private int _delay;
        private bool _active;
        private float _radius;
        private float _force;
        private string _damageDie;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.TimedExplosion; } }
        public bool active { get { return _active; } set { _active = value; } }
        public int delay { get { return _delay; } set { _delay = value; } }
        public float radius { get { return _radius; } }
        public float force { get { return _force; } }
        public string damageDie { get { return _damageDie; } }

        public TimedExplosionComponent(int entityId, int delay, float radius, float force, string damageDie)
        {
            _entityId = entityId;
            _delay = delay;
            _radius = radius;
            _force = force;
            _damageDie = damageDie;
        }
    }
}
