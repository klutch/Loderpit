using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class FlameAuraSkill : Skill
    {
        public string chanceToProc { get { return calculateChanceToProc(); } }
        public string damageDie { get { return calculateDamageDie(); } }
        public int tickDelay { get { return calculateTickDelay(); } }
        public int tickCount { get { return calculateTickCount(); } }

        public FlameAuraSkill(int entityId, int level)
            : base(SkillType.FlameAura, entityId, level, false)
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
                default: return 3;
            }
        }

        private string calculateChanceToProc()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }

        private float calculateRadius()
        {
            switch (_level)
            {
                default: return 4.5f;
            }
        }
    }
}
