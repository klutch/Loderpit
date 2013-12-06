using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class KickSkill : Skill
    {
        public string chanceToKnockback { get { return calculateChanceToKnockback(); } }
        public float knockbackForce { get { return calculateKnockbackForce(); } }

        public KickSkill(int entityId, int level)
            : base(SkillType.Kick, entityId, level, false)
        {
            _range = 1.5f;
            _baseCooldown = calculateBaseCooldown();
        }

        public override int calculateBaseCooldown()
        {
            switch (_level)
            {
                default: return 60;
            }
        }

        private float calculateKnockbackForce()
        {
            switch (_level)
            {
                default: return 200f;
            }
        }

        private string calculateChanceToKnockback()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }
    }
}
