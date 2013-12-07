using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class FatalitySkill : Skill
    {
        public FatalitySkill(int entityId, int level)
            : base(SkillType.Fatality, entityId, level, true)
        {
            _baseCooldown = 360;
            _range = 1.5f;
        }
    }

    public class ExecuteFatalitySkill : ExecuteSkill
    {
        private int _targetEntityId;

        public int targetEntityId { get { return _targetEntityId; } }

        public ExecuteFatalitySkill(Skill skill, int targetEntityId, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _targetEntityId = targetEntityId;
            _delay = 60;
        }
    }
}
