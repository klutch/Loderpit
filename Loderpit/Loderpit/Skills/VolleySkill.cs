using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class VolleySkill : Skill
    {
        public int tickDelay { get { return calculateTickDelay(); } }
        public int tickCount { get { return calculateTickCount(); } }
        public string damageDie { get { return calculateDamageDie(); } }
        public string attackDie { get { return calculateAttackDie(); } }
        public float width { get { return calculateWidth(); } }

        public VolleySkill(int entityId, int level)
            : base(SkillType.Volley, entityId, level, true)
        {
            _range = 8f;
            _baseCooldown = 360;
        }

        private int calculateTickDelay()
        {
            return 30;
        }

        private int calculateTickCount()
        {
            return 12;
        }

        private string calculateDamageDie()
        {
            switch (_level)
            {
                default: return "2d3";
            }
        }

        private string calculateAttackDie()
        {
            return "d20";
        }

        private float calculateWidth()
        {
            return 10f;
        }
    }

    public class ExecuteVolleySkill : ExecuteSkill
    {
        public ExecuteVolleySkill(Skill skill, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            VolleySkill volleySkill = skill as VolleySkill;

            _delay = volleySkill.tickDelay * volleySkill.tickCount;
        }
    }
}
