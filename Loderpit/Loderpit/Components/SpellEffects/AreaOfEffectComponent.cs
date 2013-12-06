using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components.SpellEffects
{
    public class AreaOfEffectComponent : IComponent
    {
        private int _entityId;
        private Body _sensor;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.AreaOfEffect; } }
        public Body sensor { get { return _sensor; } }

        public AreaOfEffectComponent(int entityId, Body sensor)
        {
            _entityId = entityId;
            _sensor = sensor;
        }
    }
}
