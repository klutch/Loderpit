using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class DamageShieldSpellEffect : SpellEffect, IAoESpellEffect
    {
        private int _damage;
        private float _radius;

        public int damage { get { return _damage; } }
        public float radius { get { return _radius; } }

        public DamageShieldSpellEffect(int damage, float radius)
            : base(SpellEffectType.DamageShield)
        {
            _damage = damage;
            _radius = radius;
            _affectsSelf = true;
            _affectsFriendly = true;
            _onHitByOther = (attackerId, defenderId) =>
                {
                    SystemManager.combatSystem.applySpellDamage(defenderId, _damage);
                };
        }
    }
}
