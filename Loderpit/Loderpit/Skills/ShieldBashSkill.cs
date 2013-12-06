using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class ShieldBashSkill : Skill
    {
        public string attackDie { get { return calculateAttackDie(); } }
        public string damageDie { get { return calculateDamageDie(); } }

        public ShieldBashSkill(int entityId, int level)
            : base(SkillType.ShieldBash, entityId, level, true)
        {
            _baseCooldown = 180;
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
