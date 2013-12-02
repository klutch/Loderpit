using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class IgniteProcSpellEffect : SpellEffect
    {
        private string _chanceDie;
        private string _damageDie;

        public IgniteProcSpellEffect(string chanceDie, string damageDie)
            : base(SpellEffectType.IgniteProc)
        {
            _chanceDie = chanceDie;
            _damageDie = damageDie;
            _affectsSelf = true;
            _onHitOther = (attackerId, defenderId) =>
                {
                    if (Roller.roll(_chanceDie) == 1)
                    {
                        SystemManager.spellEffectSystem.applySpellEffect(defenderId, new IgniteSpellEffect(_damageDie, 60, 6));
                    }
                };
        }
    }
}
