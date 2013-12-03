using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Formations;

namespace Loderpit.Skills
{
    public class ThrowRopeSkill : Skill
    {
        public ThrowRopeSkill(int level)
            : base(SkillType.ThrowRope, level, true)
        {
            _range = 4f;
        }
    }

    public class ExecuteThrowRopeSkill : ExecuteSkill
    {
        private Vector2 _anchor;
        private Formation _formationToRemove;

        public Vector2 anchor { get { return _anchor; } }
        public Formation formationToRemove { get { return _formationToRemove; } set { _formationToRemove = value; } }

        public ExecuteThrowRopeSkill(Skill skill, Vector2 anchor, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _anchor = anchor;
            _delay = calculateDelay();
        }

        private int calculateDelay()
        {
            switch (skill.level)
            {
                default: return 180;
            }
        }
    }
}
