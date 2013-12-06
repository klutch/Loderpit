using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class ProcComponent : IComponent
    {
        private int _entityId;
        private Action<int, int> _onHitOther;
        private Action<int, int> _onHitByOther;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Proc; } }
        public Action<int, int> onHitOther { get { return _onHitOther; } }
        public Action<int, int> onHitByOther { get { return _onHitByOther; } }

        public ProcComponent(int entityId, Action<int, int> onHitOther, Action<int, int> onHitByOther)
        {
            _entityId = entityId;
            _onHitOther = onHitOther;
            _onHitByOther = onHitByOther;
        }
    }
}
