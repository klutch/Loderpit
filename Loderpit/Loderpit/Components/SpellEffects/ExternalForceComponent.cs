using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Components.SpellEffects
{
    public class ExternalForceComponent : IComponent
    {
        private int _entityId;
        private Vector2 _force;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.ExternalForce; } }
        public Vector2 force { get { return _force; } }

        public ExternalForceComponent(int entityId, Vector2 force)
        {
            _entityId = entityId;
            _force = force;
        }
    }
}
