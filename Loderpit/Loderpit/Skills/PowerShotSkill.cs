using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class PowerShotSkill : Skill
    {
        private float _range;

        public float range { get { return _range; } }

        public PowerShotSkill(int level, float range)
            : base(SkillType.PowerShot, level, true)
        {
            _range = range;
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
        private Vector2 _target;

        public Vector2 target { get { return _target; } }

        public ExecutePowerShotSkill(Skill skill, Vector2 target)
            : base(skill)
        {
            _target = target;
            _delay = 60;
        }
    }
}
