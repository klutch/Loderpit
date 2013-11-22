using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class FactionComponent : IComponent
    {
        private int _entityId;
        private Faction _faction;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Faction; } }
        public Faction faction { get { return _faction; } set { _faction = value; } }

        public FactionComponent(int entityId, Faction faction)
        {
            _faction = faction;
        }
    }
}
