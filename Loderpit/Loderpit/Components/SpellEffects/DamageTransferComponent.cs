using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class DamageTransferComponent : IComponent
    {
        private int _entityId;
        private int _transferToEntityId;
        private float _transferPercentage;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DamageTransfer; } }
        public int transferToEntityId { get { return _transferToEntityId; } }
        public float transferPercentage { get { return _transferPercentage; } }

        public DamageTransferComponent(int entityId, int transferToEntityId, float transferPercentage)
        {
            _entityId = entityId;
            _transferToEntityId = transferToEntityId;
            _transferPercentage = transferPercentage;
        }
    }
}
