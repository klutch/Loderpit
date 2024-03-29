﻿using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class FrenzySkill : Skill
    {
        public int duration { get { return calculateDuration(); } }
        public int damageBonus { get { return calculateDamageBonus(); } }
        public int attackDelayBonus { get { return calculateAttackDelayBonus(); } }
        
        public FrenzySkill(int entityId, int level)
            : base(SkillType.Frenzy, entityId, level, true)
        {
            _baseCooldown = 900;
        }

        private int calculateDamageBonus()
        {
            switch (_level)
            {
                default: return 4;
            }
        }

        private int calculateAttackDelayBonus()
        {
            switch (_level)
            {
                default: return 30;
            }
        }

        private int calculateDuration()
        {
            switch (_level)
            {
                default: return 540;
            }
        }
    }

    public class ExecuteFrenzySkill : ExecuteSkill
    {
        public ExecuteFrenzySkill(Skill skill, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            FrenzySkill frenzySkill = skill as FrenzySkill;

            _delay = frenzySkill.duration;
        }
    }
}
