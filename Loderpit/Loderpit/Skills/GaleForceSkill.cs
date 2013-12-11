using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class GaleForceSkill : Skill
    {
        private Vector2 _windForce;

        public int damageBonus { get { return calculateDamageBonus(); } }
        public int duration { get { return calculateDuration(); } }
        public Vector2 windForce { get { return _windForce; } }

        public GaleForceSkill(int entityId, int level, Vector2 windForce)
            : base(SkillType.GaleForce, entityId, level, true)
        {
            _windForce = windForce;
            _baseCooldown = 1080;
        }

        private int calculateDuration()
        {
            switch (_level)
            {
                default: return 540;
            }
        }

        private int calculateDamageBonus()
        {
            switch (_level)
            {
                default: return 3;
            }
        }
    }
}
