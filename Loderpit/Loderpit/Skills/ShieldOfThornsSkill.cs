using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class ShieldOfThornsSkill : Skill
    {
        public ShieldOfThornsSkill(int level)
            : base(SkillType.ShieldOfThorns, level, false)
        {
            _passiveSpellEffects.Add(
                new DamageShieldSpellEffect(calculateDamage(), calculateRadius()));
        }

        private int calculateDamage()
        {
            switch (_level)
            {
                case 1: return 2;
                case 2: return 4;
                default: return 1;
            }
        }

        private float calculateRadius()
        {
            switch (_level)
            {
                default: return 2.5f;
            }
        }
    }
}
