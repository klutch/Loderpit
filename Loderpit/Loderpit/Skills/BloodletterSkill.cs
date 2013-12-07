using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class BloodletterSkill : Skill
    {
        public string chanceToProc { get { return calculateChanceToProc(); } }
        public string bleedingDamageDie { get { return calculateBleedingDamageDie(); } }
        public int tickCount { get { return calculateTickCount(); } }
        public int tickDelay { get { return 60; } }

        public BloodletterSkill(int entityId, int level) :
            base(SkillType.Bloodletter, entityId, level, false)
        {
        }

        private string calculateChanceToProc()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }

        private string calculateBleedingDamageDie()
        {
            switch (_level)
            {
                default: return "d3";
            }
        }

        private int calculateTickCount()
        {
            switch (_level)
            {
                default: return 4;
            }
        }
    }
}
