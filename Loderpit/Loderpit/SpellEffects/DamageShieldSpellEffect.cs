using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class DamageShieldSpellEffect : SpellEffect
    {
        public DamageShieldSpellEffect(int damage, float radius)
            : base(SpellEffectType.DamageShield)
        {
            _damage = damage;
            _radius = radius;
            _affectsSelf = true;
            _affectsFriendly = true;
            _onHitByOther = (attackerId, defenderId) =>
                {
                    SystemManager.combatSystem.applySpellDamage(attackerId, defenderId, _damage);
                };
        }
    }
}
