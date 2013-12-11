using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class DotDamageModifierComponent : IComponent
    {
        private int _entityId;
        private DamageType _damageTypeToModify;
        private int _amount;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.DotDamageModifier; } }
        public DamageType damageTypeToModifier { get { return _damageTypeToModify; } }
        public int amount { get { return _amount; } }

        public DotDamageModifierComponent(int entityId, DamageType damageTypeToModify, int amount)
        {
            _entityId = entityId;
            _damageTypeToModify = damageTypeToModify;
            _amount = amount;
        }
    }
}
