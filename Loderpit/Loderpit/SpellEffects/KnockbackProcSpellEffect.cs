using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class KnockbackProcSpellEffect : SpellEffect
    {
        private float _strength;

        public KnockbackProcSpellEffect(float strength)
            : base(SpellEffectType.KnockbackProc)
        {
            _strength = strength;
            _affectsHostile = true;
            _onHitOther = (attackerId, defenderId) => { SystemManager.combatSystem.applyKnockback(attackerId, defenderId, _strength); };
        }
    }
}
