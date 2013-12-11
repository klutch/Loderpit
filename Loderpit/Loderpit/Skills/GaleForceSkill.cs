using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loderpit.Skills
{
    public class GaleForceSkill : Skill
    {
        public int damageBonus { get { return calculateDamageBonus(); } }
        public int duration { get { return calculateDuration(); } }

        public GaleForceSkill(int entityId, int level)
            : base(SkillType.GaleForce, entityId, level, true)
        {
            _baseCooldown = 1080;
        }

        private int calculateDuration()
        {
            switch (_level)
            {
                default: return 540;
            }
        }

        private int calculateDamageBonus()
        {
            switch (_level)
            {
                default: return 3;
            }
        }
    }
}
