using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class DispelSkill : Skill
    {
        public DispelSkill(int entityId, int level)
            : base(SkillType.Dispel, entityId, level, true)
        {
            _range = 4f;
            _baseCooldown = 900;
        }
    }

    public class ExecuteDispelSkill : ExecuteSkill
    {
        private int _targetId;

        public int targetId { get { return _targetId; } }

        public ExecuteDispelSkill(Skill skill, int targetId, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _targetId = targetId;
        }
    }
}
