using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class KickSkill : Skill
    {
        private float _range;

        public float range { get { return _range; } }

        public KickSkill(int level)
            : base(SkillType.Kick, level, false)
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
                default: return 100f;
            }
        }

        // Calculates the attack die
        public string calculateAttackDie()
        {
            switch (_level)
            {
                default: return "d20";
            }
        }

        // Calculates the hit die
        public string calculateHitDie()
        {
            switch (_level)
            {
                default: return "1d5";
            }
        }
    }
}
