using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class TrackEntityPositionComponent : IComponent
    {
        private int _entityId;
        private int _targetEntityId;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.TrackEntityPosition; } }
        public int targetEntityId { get { return _targetEntityId; } }

        public TrackEntityPositionComponent(int entityId, int targetEntityId)
        {
            _entityId = entityId;
            _targetEntityId = targetEntityId;
        }
    }
}
