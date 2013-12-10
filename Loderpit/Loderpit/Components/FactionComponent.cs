using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class FactionComponent : IComponent
    {
        private int _entityId;
        private Faction _faction;
        private Faction _hostileFaction;
        private List<Faction> _attackableFactions;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Faction; } }
        public Faction faction { get { return _faction; } }
        public Faction hostileFaction { get { return _hostileFaction; } }
        public List<Faction> attackableFactions { get { return _attackableFactions; } }

        public FactionComponent(int entityId, Faction faction, Faction hostileFaction, List<Faction> attackableFactions = null)
        {
            _faction = faction;
            _hostileFaction = hostileFaction;
            _attackableFactions = attackableFactions ?? new List<Faction>(new[] { Faction.Neutral, hostileFaction });
        }
    }
}
