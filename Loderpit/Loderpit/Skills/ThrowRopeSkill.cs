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
            _range = 6f;
        }
    }

    public class ExecuteThrowRopeSkill : ExecuteSkill
    {
        private Vector2 _anchor;
        private Formation _formationToRemove;

        public Vector2 anchor { get { return _anchor; } }
        public Formation formationToRemove { get { return _formationToRemove; } set { _formationToRemove = value; } }

        public ExecuteThrowRopeSkill(Skill skill, Vector2 anchor, Func<bool> isDelayConditionMetCallback)
            : base(skill)
        {
            _anchor = anchor;
            _delay = 180;
            _isDelayConditionMetCallback = isDelayConditionMetCallback;
        }
    }
}
