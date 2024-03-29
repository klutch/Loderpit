﻿using System;
using System.Collections.Generic;

namespace Loderpit.Components.SpellEffects
{
    public class StatModifierComponent : IComponent
    {
        private int _entityId;
        private int _attackDieMod;
        private int _damageDieMod;
        private int _strengthMod;
        private int _dexterityMod;
        private int _intelligenceMod;
        private int _maxHpMod;
        private int _armorClassMod;
        private int _attackDelayMod;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.StatModifier; } }
        public int attackDieMod { get { return _attackDieMod; } set { _attackDieMod = value; } }
        public int damageDieMod { get { return _damageDieMod; } set { _damageDieMod = value; } }
        public int strengthMod { get { return _strengthMod; } set { _strengthMod = value; } }
        public int dexterityMod { get { return _dexterityMod; } set { _dexterityMod = value; } }
        public int intelligenceMod { get { return _intelligenceMod; } set { _intelligenceMod = value; } }
        public int maxHpMod { get { return _maxHpMod; } set { _maxHpMod = value; } }
        public int armorClassMod { get { return _armorClassMod; } set { _armorClassMod = value; } }
        public int attackDelayMod { get { return _attackDelayMod; } set { _attackDelayMod = value; } }

        public StatModifierComponent(int entityId)
        {
            _entityId = entityId;
        }
    }
}
