using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class PowerShotSkill : Skill
    {
        public PowerShotSkill(int level)
            : base(SkillType.PowerShot, level, true)
        {
            _baseCooldown = 360;
        }

        public int calculateExtraDamage()
        {
            switch (_level)
            {
                default: return 10;
            }
        }
    }
}
