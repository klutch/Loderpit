using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class KnockbackProcSpellEffect : SpellEffect
    {
        private float _strength;
        private Vector2 _directionNormal;

        public KnockbackProcSpellEffect(int sourceEntityId, float strength, Vector2 directionNormal)
            : base(SpellEffectType.KnockbackProc, sourceEntityId)
        {
            _strength = strength;
            _directionNormal = directionNormal;
            _affectsSelf = true;
            _onHitOther = (attackerId, defenderId) => { SystemManager.combatSystem.applyKnockback(attackerId, defenderId, _strength, _directionNormal); };
        }
    }
}
