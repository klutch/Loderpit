using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class KickSkill : Skill
    {
        public KickSkill(int entityId, int level)
            : base(SkillType.Kick, entityId, level, false)
        {
            _range = 1.5f;
        }

        // Calculates the cooldown duration
        public override int calculateBaseCooldown()
        {
            switch (_level)
            {
                default: return 60;
            }
        }

        // Calculates the knock back force
        public float calculateKnockbackForce()
        {
            switch (_level)
            {
                default: return 200f;
            }
        }
    }
}
