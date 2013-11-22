using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Joints;
using Loderpit.Formations;

namespace Loderpit.Components
{
    public class RopeGrabComponent : IComponent
    {
        private int _entityId;
        private RopeComponent _ropeComponent;
        private RevoluteJoint _joint;
        private float _progress;
        private LimitedRangeFormation _formationToRemove;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.RopeGrab; } }
        public RevoluteJoint joint { get { return _joint; } set { _joint = value; } }
        public RopeComponent ropeComponent { get { return _ropeComponent; } }
        public float progress { get { return _progress; } set { _progress = value; } }
        public LimitedRangeFormation formationToRemove { get { return _formationToRemove; } set { _formationToRemove = value; } }

        public RopeGrabComponent(int entityId, RopeComponent ropeComponent, RevoluteJoint joint, float progress)
        {
            _entityId = entityId;
            _ropeComponent = ropeComponent;
            _joint = joint;
            _progress = progress;
        }
    }
}
