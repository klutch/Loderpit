using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loderpit.Skills
{
    public class BattleDroneSkill : Skill
    {
        public int droneCount { get { return 2; } }
        public string damageDie { get { return "3d3"; } }
        public int maxHp { get { return 10; } }

        public BattleDroneSkill(int entityId, int level)
            : base(SkillType.BattleDrone, entityId, level, false)
        {
            _baseCooldown = 1280;
        }
    }
}
