using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class BattleCrySkill : Skill
    {
        public int damageBonus { get { return 2; } }
        public int attackDelayBonus { get { return 8; } }

        public BattleCrySkill(int entityId, int level)
            : base(SkillType.BattleCry, entityId, level, false)
        {
            _range = 2.5f;
        }
    }
}
