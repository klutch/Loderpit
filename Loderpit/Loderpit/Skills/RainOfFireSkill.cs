using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class RainOfFireSkill : Skill
    {
        public string damageDie { get { return calculateDamageDie(); } }
        public int tickCount { get { return calculateTickCount(); } }
        public int tickDelay { get { return 30; } }
        public float width { get { return calculateWidth(); } }

        public RainOfFireSkill(int entityId, int level)
            : base(SkillType.RainOfFire, entityId, level, true)
        {
            _baseCooldown = 360;
            _range = 8f;
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "d3";
            }
        }

        private int calculateTickCount()
        {
            switch (_level)
            {
                default: return 6;
            }
        }

        private float calculateWidth()
        {
            switch (_level)
            {
                default: return 10f;
            }
        }
    }

    public class ExecuteRainOfFireSkill : ExecuteSkill
    {
        public ExecuteRainOfFireSkill(Skill skill, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            RainOfFireSkill rainOfFireSkill = skill as RainOfFireSkill;

            _delay = rainOfFireSkill.tickCount * rainOfFireSkill.tickDelay;
        }
    }
}
