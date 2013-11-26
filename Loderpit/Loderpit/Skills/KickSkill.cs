﻿using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class KickSkill : Skill
    {
        public KickSkill(int level)
            : base(SkillType.Kick, level, false)
        {
            _range = 1.5f;
            _onActivateSpellEffects.Add(new KnockbackProcSpellEffect(calculateKnockbackForce()));
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

        // Calculates the attack die
        public override string calculateAttackDie()
        {
            switch (_level)
            {
                default: return "d20";
            }
        }

        // Calculates the hit die
        public override string calculateHitDie()
        {
            switch (_level)
            {
                default: return "1d5";
            }
        }
    }
}