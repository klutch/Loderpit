using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class BlockSkill : Skill
    {
        public BlockSkill(int entityId, int level)
            : base(SkillType.Block, entityId, level, false)
        {
        }

        public int calculateNumBlockableEntities()
        {
            switch (_level)
            {
                default: return 1;
            }
        }
    }
}
