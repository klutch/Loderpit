using System;
using System.Collections.Generic;

namespace Loderpit.SpellEffects
{
    public class StatBuffSpellEffect : SpellEffect, IAoESpellEffect
    {
        private int _attackRollModifier;
        private int _hitRollModifier;
        private int _armorClassModifier;
        private int _maxHpModifier;
        private int _strengthModifier;
        private int _dexterityModifier;
        private int _intelligenceModifier;
        private float _radius;

        public int attackRollModifier { get { return _attackRollModifier; } }
        public int hitRollModifier { get { return _hitRollModifier; } }
        public int armorClassModifier { get { return _armorClassModifier; } }
        public int maxHpModifier { get { return _maxHpModifier; } }
        public int strengthModifier { get { return _strengthModifier; } }
        public int dexterityModifier { get { return _dexterityModifier; } }
        public int intelligenceModifier { get { return _intelligenceModifier; } }
        public float radius { get { return _radius; } }

        public StatBuffSpellEffect(
            int sourceEntityId,
            int attackRollModifier,
            int hitRollModifier,
            int armorClassModifier,
            int maxHpModifier,
            int strengthModifier,
            int dexterityModifier,
            int intelligenceModifier,
            bool affectsSelf,
            bool affectsFriendly,
            bool affectsHostile,
            float radius)
            : base(SpellEffectType.StatBuff, sourceEntityId)
        {
            _attackRollModifier = attackRollModifier;
            _hitRollModifier = hitRollModifier;
            _armorClassModifier = armorClassModifier;
            _maxHpModifier = maxHpModifier;
            _strengthModifier = strengthModifier;
            _dexterityModifier = dexterityModifier;
            _intelligenceModifier = intelligenceModifier;
            _affectsSelf = affectsSelf;
            _affectsFriendly = affectsFriendly;
            _affectsHostile = affectsHostile;
            _radius = radius;
        }
    }
}
