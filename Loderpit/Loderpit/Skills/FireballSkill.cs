using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class FireballSkill : Skill
    {
        public string burnChanceDie { get { return calculateBurnChanceDie(); } }
        public string burnDamageDie { get { return calculateBurnDamageDie(); } }
        public string explosionDamageDie { get { return calculateExplosionDamageDie(); } }
        public float explosionRadius { get { return calculateExplosionRadius(); } }
        public float explosionForce { get { return calculateExplosionForce(); } }
        public int burnTickCount { get { return calculateBurnTickCount(); } }
        public int burnTickDelay { get { return calculateBurnTickDelay(); } }

        public FireballSkill(int entityId, int level)
            : base(SkillType.Fireball, entityId, level, true)
        {
            _baseCooldown = 180;
            _range = 8f;
        }

        private int calculateBurnTickCount()
        {
            switch (_level)
            {
                default: return 4;
            }
        }

        private int calculateBurnTickDelay()
        {
            switch (_level)
            {
                default: return 60;
            }
        }

        private float calculateExplosionRadius()
        {
            switch (_level)
            {
                default: return 4f;
            }
        }

        private string calculateBurnChanceDie()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }

        private string calculateBurnDamageDie()
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
