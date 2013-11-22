using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class ThrowRopeSkill : Skill
    {
        private float _range;

        public float range { get { return _range; } }

        public ThrowRopeSkill(int level, float range)
            : base(SkillType.ThrowRope, level, true)
        {
            _range = range;
        }
    }
}
