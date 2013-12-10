using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class PiercingSkill : Skill
    {
        public string bleedingChanceToProc { get { return calculateBleedingChanceToProc(); } }
        public string bleedingDamageDie { get { return calculateBleedingDamageDie(); } }
        public int bleedingTickDelay { get { return 30; } }
        public int bleedingTickCount { get { return 6; } }

        public PiercingSkill(int entityId, int level)
            : base(SkillType.Piercing, entityId, level, false)
        {
            _range = 8f;
        }

        private string calculateBleedingChanceToProc()
        {
            return "d1";
        }

        private string calculateBleedingDamageDie()
        {
            return "d2";
        }
    }

    public class ExecutePiercingSkill : ExecuteSkill
    {
        private int _defenderId;

        public ExecutePiercingSkill(Skill skill, int defenderId)
            : base(skill)
        {
            _defenderId = defenderId;
        }
    }
}
