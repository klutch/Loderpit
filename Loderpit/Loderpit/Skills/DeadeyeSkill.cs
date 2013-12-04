using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class DeadeyeSkill : Skill
    {
        public DeadeyeSkill(int entityId, int level)
            : base(SkillType.Deadeye, entityId, level, false)
        {
            _passiveSpellEffects.Add(new StatBuffSpellEffect(entityId, calculateAttackRollModifier(), 0, 0, 0, 0, 0, 0, true, true, false, calculateRadius()));
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
