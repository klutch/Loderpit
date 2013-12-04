using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public class FireballSkill : Skill
    {
        public string igniteChanceDie { get { return calculateIgniteChanceDie(); } }
        public string igniteDamageDie { get { return calculateIgniteDamageDie(); } }
        public string explosionDamageDie { get { return calculateExplosionDamageDie(); } }
        public float explosionRadius { get { return calculateExplosionRadius(); } }
        public float explosionForce { get { return calculateExplosionForce(); } }

        public FireballSkill(int entityId, int level)
            : base(SkillType.Fireball, entityId, level, true)
        {
            _baseCooldown = 180;
            _range = 8f;
        }

        private float calculateExplosionRadius()
        {
            switch (_level)
            {
                default: return 4f;
            }
        }

        private string calculateIgniteChanceDie()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }

        private string calculateIgniteDamageDie()
        {
            switch (_level)
            {
                default: return "d3";
            }
        }

        private string calculateExplosionDamageDie()
        {
            switch (_level)
            {
                default: return "d4";
            }
        }

        private float calculateExplosionForce()
        {
            switch (_level)
            {
                default: return 100f;
            }
        }
    }

    public class ExecuteFireballSkill : ExecuteSkill
    {
        private Vector2 _target;

        public Vector2 target { get { return _target; } }

        public ExecuteFireballSkill(Skill skill, Vector2 target, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _delay = 60;
            _target = target;
        }
    }
}
