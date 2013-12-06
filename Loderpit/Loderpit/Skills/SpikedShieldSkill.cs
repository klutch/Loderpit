using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class SpikedShieldSkill : Skill
    {
        public SpikedShieldSkill(int entityId, int level)
            : base(SkillType.SpikedShield, entityId, level, false)
        {
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "d2";
            }
        }

        private float calculateRadius()
        {
            switch (_level)
            {
                default: return 2f;
            }
        }
    }
}
