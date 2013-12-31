using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class MeleeAttackSkill : Skill
    {
        public MeleeAttackSkill(int entityId, int level)
            : base(SkillType.MeleeAttack, entityId, level, true, false)
        {
            _range = 1.2f;
        }
    }
}
