using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class ShieldOfThornsSkill : Skill
    {
        public string damageDie { get { return calculateDamageDie(); } }

        public ShieldOfThornsSkill(int entityId, int level)
            : base(SkillType.ShieldOfThorns, entityId, level, false)
        {
            _range = calculateRadius();
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "d2";
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
