using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class KickSkill : Skill
    {
        public KickSkill(int level)
            : base(SkillType.Kick, level, false)
        {
            _range = 1.5f;
            _onActivateSpellEffects.Add(new KnockbackProcSpellEffect(calculateKnockbackForce(), new Vector2(1f, -0.5f)));
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
