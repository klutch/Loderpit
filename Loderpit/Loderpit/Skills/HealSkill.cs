using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loderpit.Skills
{
    public class HealSkill : Skill
    {
        public string healDie { get { return calculateHealDie(); } }

        public HealSkill(int entityId, int level)
            : base(SkillType.Heal, entityId, level, false)
        {
            _baseCooldown = 180;
            _range = 8f;
        }

        private string calculateHealDie()
        {
            switch (_level)
            {
                default: return "1d3";
            }
        }
    }
}
