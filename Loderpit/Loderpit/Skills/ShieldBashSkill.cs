using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class ShieldBashSkill : Skill
    {
        private Vector2 _knockbackNormal;

        public string attackDie { get { return calculateAttackDie(); } }
        public string damageDie { get { return calculateDamageDie(); } }
        public float knockbackForce { get { return calculateKnockbackStrength(); } }
        public Vector2 knockbackNormal { get { return _knockbackNormal; } }

        public ShieldBashSkill(int entityId, int level, Vector2 knockbackNormal)
            : base(SkillType.ShieldBash, entityId, level, true)
        {
            _baseCooldown = 180;
            _knockbackNormal = knockbackNormal;
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
