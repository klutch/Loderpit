using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class MeleeAttackSkill : Skill
    {
        public MeleeAttackSkill(int level)
            : base(SkillType.MeleeAttack, level, true)
        {
            _range = 1.2f;
        }
    }
}
