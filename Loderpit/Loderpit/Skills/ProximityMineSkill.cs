using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class ProximityMineSkill : Skill
    {
        public float explosionRadius { get { return calculateExplosionRadius(); } }
        public string damageDie { get { return calculateDamageDie(); } }
        public float explosionForce { get { return calculateExplosionForce(); } }

        public ProximityMineSkill(int entityId, int level)
            : base(SkillType.ProximityMine, entityId, level, true)
        {
            _baseCooldown = 180;
            _range = 10f;
        }

        private float calculateExplosionRadius()
        {
            switch (_level)
            {
                default: return 3f;
            }
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "2d3";
            }
        }

        private float calculateExplosionForce()
        {
            switch (_level)
            {
                default: return 200f;
            }
        }
    }

    public class ExecuteProximityMineSkill : ExecuteSkill
    {
        private Vector2 _target;

        public Vector2 target { get { return _target; } }

        public ExecuteProximityMineSkill(ProximityMineSkill skill, Vector2 target, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _target = target;
            _delay = 120;
        }
    }
}
