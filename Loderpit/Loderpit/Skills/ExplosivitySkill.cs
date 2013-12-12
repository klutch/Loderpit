using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class ExplosivitySkill : Skill
    {
        private int _explosivitySpellId;

        public string explosionChanceToProc { get { return calculateExplosionChanceToProc(); } }
        public string explosionDamageDie { get { return calculateExplosionDamageDie(); } }
        public float explosionForce { get { return 200f; } }
        public string burningChanceToProc { get { return calculateBurningChanceToProc(); } }
        public string burningDamageDie { get { return calculateBurningDamageDie(); } }
        public int burningTickDelay { get { return 60; } }
        public int burningTickCount { get { return 3; } }
        public int explosivitySpellId { get { return _explosivitySpellId; } set { _explosivitySpellId = value; } }

        public ExplosivitySkill(int entityId, int level)
            : base(SkillType.Explosivity, entityId, level, false)
        {
            _range = calculateRadius();
        }

        private string calculateExplosionChanceToProc()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }

        private string calculateExplosionDamageDie()
        {
            switch (_level)
            {
                default: return "2d3";
            }
        }

        private string calculateBurningChanceToProc()
        {
            switch (_level)
            {
                default: return "d1";
            }
        }

        private string calculateBurningDamageDie()
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
