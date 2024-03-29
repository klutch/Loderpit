﻿using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class PowerSwingSkill : Skill
    {
        public PowerSwingSkill(int entityId, int level)
            : base(SkillType.PowerSwing, entityId, level, true)
        {
            _range = 1.2f;
            _baseCooldown = 360;
        }

        public int calculateExtraDamage()
        {
            switch (_level)
            {
                default: return 10;
            }
        }
    }

    public class ExecutePowerSwingSkill : ExecuteSkill
    {
        private int _defenderId;

        public int defenderId { get { return _defenderId; } }

        public ExecutePowerSwingSkill(Skill skill, int defenderId, Func<bool> isDelayConditionMetCallback)
            : base(skill)
        {
            _defenderId = defenderId;
            _delay = 60;
            _isDelayConditionMetCallback = isDelayConditionMetCallback;
        }
    }
}
