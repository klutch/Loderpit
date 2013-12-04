using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class ShieldBashSkill : Skill
    {
        public string attackDie { get { return calculateAttackDie(); } }
        public string damageDie { get { return calculateDamageDie(); } }

        public ShieldBashSkill(int entityId, int level)
            : base(SkillType.ShieldBash, entityId, level, true)
        {
            _baseCooldown = 180;
            _onActivateSpellEffects.Add(new KnockbackProcSpellEffect(entityId, calculateKnockbackStrength(), new Vector2(1, -0.5f)));
        }

        private float calculateKnockbackStrength()
        {
            switch (_level)
            {
                default: return 200f;
            }
        }

        private string calculateAttackDie()
        {
            switch (_level)
            {
                default: return "d20";
            }
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "d4";
            }
        }
    }
}
