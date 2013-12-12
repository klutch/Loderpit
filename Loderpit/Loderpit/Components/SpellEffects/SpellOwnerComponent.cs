using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class SpellOwnerComponent : IComponent
    {
        private int _entityId;
        private int _ownerId;
        
        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.SpellOwner; } }
        public int ownerId { get { return _ownerId; } }

        public SpellOwnerComponent(int entityId, int ownerId)
        {
            _entityId = entityId;
            _ownerId = ownerId;
        }
    }
}
