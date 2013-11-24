using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class FactionComponent : IComponent
    {
        private int _entityId;
        private Faction _faction;
        private Faction _hostileFaction;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Faction; } }
        public Faction faction { get { return _faction; } }
        public Faction hostileFaction { get { return _hostileFaction; } }

        public FactionComponent(int entityId, Faction faction, Faction hostileFaction)
        {
            _faction = faction;
            _hostileFaction = hostileFaction;
        }
    }
}
