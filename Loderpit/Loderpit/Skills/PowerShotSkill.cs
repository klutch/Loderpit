﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class PowerShotSkill : Skill
    {
        public PowerShotSkill(int level)
            : base(SkillType.PowerShot, level, true)
        {
            _range = 10f;
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

    public class ExecutePowerShotSkill : ExecuteSkill
    {
        private int _defenderId;

        public int defenderId { get { return _defenderId; } }

        public ExecutePowerShotSkill(Skill skill, int defenderId)
            : base(skill)
        {
            _defenderId = defenderId;
            _delay = 60;
        }
    }
}
