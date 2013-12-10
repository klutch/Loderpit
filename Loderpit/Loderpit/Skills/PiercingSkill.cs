using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class PiercingSkill : Skill
    {
        public PiercingSkill(int entityId, int level)
            : base(SkillType.Piercing, entityId, level, false)
        {
            _range = 8f;
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
