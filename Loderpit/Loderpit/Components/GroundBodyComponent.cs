using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using FarseerPhysics.Dynamics;

namespace Loderpit.Components
{
    public class GroundBodyComponent : IComponent
    {
        private int _entityId;
        private Body _body;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.GroundBody; } }

        public GroundBodyComponent(int entityId, Body body)
        {
            _entityId = entityId;
            _body = body;
        }
    }
}
