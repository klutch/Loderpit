using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class ShieldBashSkill : Skill
    {
        public string attackDie { get { return calculateAttackDie(); } }
        public string damageDie { get { return calculateDamageDie(); } }

        public ShieldBashSkill(int level)
            : base(SkillType.ShieldBash, level, true)
        {
            _onActivateSpellEffects.Add(new KnockbackProcSpellEffect(calculateKnockbackStrength(), new Vector2(1, -0.5f)));
        }

        private float calculateKnockbackStrength()
        {
            switch (_level)
            {
                default: return 200f;
            }
        }

        private string calculateAttackDie()
        {
            switch (_level)
            {
                default: return "d20";
            }
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "d4";
            }
        }
    }
}
