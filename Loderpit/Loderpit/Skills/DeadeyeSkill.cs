using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class DeadeyeSkill : Skill
    {
        public int attackDieMod { get { return calculateAttackRollModifier(); } }

        public DeadeyeSkill(int entityId, int level)
            : base(SkillType.Deadeye, entityId, level, false)
        {
            _range = calculateRadius();
        }

        private int calculateAttackRollModifier()
        {
            switch (_level)
            {
                default: return 2;
            }
        }

        private float calculateRadius()
        {
            switch (_level)
            {
                default: return 2.5f;
            }
        }
    }
}
