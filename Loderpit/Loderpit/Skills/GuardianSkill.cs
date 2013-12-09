using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class GuardianSkill : Skill
    {
        public float transferPercentage { get { return calculateTransferPercentage(); } }

        public GuardianSkill(int entityId, int level)
            : base(SkillType.Guardian, entityId, level, false)
        {
            _range = calculateRange();
        }

        private float calculateRange()
        {
            switch (_level)
            {
                default: return 2.5f;
            }
        }

        private float calculateTransferPercentage()
        {
            switch (_level)
            {
                default: return 0.5f;
            }
        }
    }
}
