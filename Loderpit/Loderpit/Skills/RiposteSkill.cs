using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class RiposteSkill : Skill
    {
        public string chanceToRiposte { get { return "d1"; } }
        public int timeToLive { get { return 180; } }

        public RiposteSkill(int entityId, int level)
            : base(SkillType.Riposte, entityId, level, true)
        {
            _baseCooldown = 360;
        }
    }
}
