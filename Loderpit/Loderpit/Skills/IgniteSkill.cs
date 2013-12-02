using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class IgniteSkill : Skill
    {
        public IgniteSkill(int level)
            : base(SkillType.Ignite, level, false)
        {
            _passiveSpellEffects.Add(new IgniteProcSpellEffect(calculateChanceDie(), calculateDamageDie()));
        }

        private string calculateChanceDie()
        {
            switch (_level)
            {
                default: return "d6";
            }
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }
    }
}
