using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class PowerShotSkill : Skill
    {
        public PowerShotSkill(int level)
            : base(SkillType.PowerShot, level, true)
        {
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
