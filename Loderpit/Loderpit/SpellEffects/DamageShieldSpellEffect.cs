using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class DamageShieldSpellEffect : SpellEffect
    {
        private int _damage;

        public DamageShieldSpellEffect(int damage)
            : base(SpellEffectType.DamageShield)
        {
            _damage = damage;

            _affectsSelf = true;
            _affectsFriendly = true;
            _onHitByOther = (attackerId, defenderId) =>
                {
                    SystemManager.combatSystem.applySpellDamage(attackerId, defenderId, _damage);
                };
        }
    }
}
