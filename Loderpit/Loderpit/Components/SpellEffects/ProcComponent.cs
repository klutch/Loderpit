using System;
using System.Collections.Generic;
using Loderpit.Skills;

namespace Loderpit.Components.SpellEffects
{
    public class ProcComponent : IComponent
    {
        private int _entityId;
        private Action<Skill, int, int> _onHitOther;
        private Action<Skill, int, int> _onHitByOther;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Proc; } }
        public Action<Skill, int, int> onHitOther { get { return _onHitOther; } }
        public Action<Skill, int, int> onHitByOther { get { return _onHitByOther; } }

        public ProcComponent(int entityId, Action<Skill, int, int> onHitOther, Action<Skill, int, int> onHitByOther)
        {
            _entityId = entityId;
            _onHitOther = onHitOther;
            _onHitByOther = onHitByOther;
        }
    }
}
