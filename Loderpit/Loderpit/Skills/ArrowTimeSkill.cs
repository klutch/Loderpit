using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class ArrowTimeSkill : Skill
    {
        public int timeToLive { get { return calculateTimeToLive(); } }
        public string attackDie { get { return calculateAttackDie(); } }
        public string damageDie { get { return calculateDamageDie(); } }

        public ArrowTimeSkill(int entityId, int level)
            : base(SkillType.ArrowTime, entityId, level, true)
        {
        }

        private int calculateTimeToLive()
        {
            switch (_level)
            {
                default: return 120;
            }
        }

        private string calculateAttackDie()
        {
            switch (_level)
            {
                default: return "9999";
            }
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "4d3";
            }
        }
    }
}
