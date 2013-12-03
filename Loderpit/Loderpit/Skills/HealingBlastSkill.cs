using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class HealingBlastSkill : Skill
    {
        public string healDie { get { return calculateHealDie(); } }

        public HealingBlastSkill(int level)
            : base(SkillType.HealingBlast, level, true)
        {
            _baseCooldown = 180;
            _range = 8f;
        }

        private string calculateHealDie()
        {
            switch (_level)
            {
                default: return "2d3";
            }
        }
    }

    public class ExecuteHealingBlastSkill : ExecuteSkill
    {
        private int _targetId;

        public int targetId { get { return _targetId; } }

        public ExecuteHealingBlastSkill(Skill skill, int targetId, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _targetId = targetId;
            _delay = 60;
        }
    }
}
