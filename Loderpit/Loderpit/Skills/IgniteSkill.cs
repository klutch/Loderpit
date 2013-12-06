using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class IgniteSkill : Skill
    {
        public int tickDelay { get { return calculateTickDelay(); } }
        public int tickCount { get { return calculateTickCount(); } }

        public IgniteSkill(int entityId, int level)
            : base(SkillType.Ignite, entityId, level, false)
        {
        }

        private int calculateTickDelay()
        {
            switch (_level)
            {
                default: return 60;
            }
        }

        private int calculateTickCount()
        {
            switch (_level)
            {
                default: return 4;
            }
        }

        private string calculateChanceDie()
        {
            switch (_level)
            {
                default: return "d6";
            }
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }
    }
}
