using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class ServoBotSkill : Skill
    {
        public int maxHp { get { return 10; } }

        public ServoBotSkill(int entityId, int level)
            : base(SkillType.ServoBot, entityId, level, true)
        {
            _baseCooldown = 1080;
        }
    }
}
