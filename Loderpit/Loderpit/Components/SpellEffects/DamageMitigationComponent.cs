using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class DamageMitigationComponent : IComponent
    {
        private int _entityId;
        private float _mitigationPercentage;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DamageMitigation; } }
        public float mitigationPercentage { get { return _mitigationPercentage; } }

        public DamageMitigationComponent(int entityId, float mitigationPercentage)
        {
            _entityId = entityId;
            _mitigationPercentage = mitigationPercentage;
        }
    }
}
