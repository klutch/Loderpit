using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class SpikedShieldSkill : Skill
    {
        public SpikedShieldSkill(int entityId, int level)
            : base(SkillType.SpikedShield, entityId, level, false)
        {
            _passiveSpellEffects.Add(new DamageAuraSpellEffect(entityId, calculateDamageDie(), calculateRadius(), 60, 1, false, false, true, true));
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
                default: return 2f;
            }
        }
    }
}
