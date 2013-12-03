using System;
using System.Collections.Generic;

namespace Loderpit.SpellEffects
{
    public class ExplosionProcSpellEffect : SpellEffect
    {
        private string _chanceDie;
        private string _damageDie;

        public ExplosionProcSpellEffect(string chanceDie, string damageDie)
            : base(SpellEffectType.ExplosionProc)
        {
            _chanceDie = chanceDie;
            _damageDie = damageDie;
        }
    }
}
