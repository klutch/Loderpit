using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class RegenerationSkill : Skill
    {
        public string healDie { get { return "2"; } }
        public int tickDelay { get { return 60; } }

        public RegenerationSkill(int entityId, int level)
            : base(SkillType.Regeneration, entityId, level, false)
        {
            _range = 8f;
        }
    }
}
