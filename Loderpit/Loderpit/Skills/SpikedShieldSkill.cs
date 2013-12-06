using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class SpikedShieldSkill : Skill
    {
        public string damageDie { get { return calculateDamageDie(); } }

        public SpikedShieldSkill(int entityId, int level)
            : base(SkillType.SpikedShield, entityId, level, false)
        {
            _range = calculateRadius();
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
