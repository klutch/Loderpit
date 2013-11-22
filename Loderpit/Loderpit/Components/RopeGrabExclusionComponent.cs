using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class RopeGrabExclusionComponent : IComponent
    {
        private int _entityId;
        private List<RopeComponent> _excludedRopeComponents;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.RopeGrabExclusion; } }
        public List<RopeComponent> excludedRopeComponents { get { return _excludedRopeComponents; } }

        public RopeGrabExclusionComponent(int entityId)
        {
            _entityId = entityId;
            _excludedRopeComponents = new List<RopeComponent>();
        }
    }
}
