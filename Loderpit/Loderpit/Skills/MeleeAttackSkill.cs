using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class MeleeAttackSkill : Skill
    {
        private float _range;

        public float range { get { return _range; } }

        public MeleeAttackSkill(int level, float range)
            : base(SkillType.MeleeAttack, level, true)
        {
            _range = range;
        }
    }
}
