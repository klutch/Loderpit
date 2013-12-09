using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class DamageTransferComponent : IComponent
    {
        private int _entityId;
        private float _transferPercentage;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DamageTransfer; } }
        public float transferPercentage { get { return _transferPercentage; } }

        public DamageTransferComponent(int entityId, float transferPercentage)
        {
            _entityId = entityId;
            _transferPercentage = transferPercentage;
        }
    }
}
