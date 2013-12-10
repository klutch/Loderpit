using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class RiposteComponent : IComponent
    {
        private int _entityId;
        private string _chanceToRiposte;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Riposte; } }
        public string chanceToRiposte { get { return _chanceToRiposte; } }

        public RiposteComponent(int entityId, string chanceToRiposte)
        {
            _entityId = entityId;
            _chanceToRiposte = chanceToRiposte;
        }
    }
}
