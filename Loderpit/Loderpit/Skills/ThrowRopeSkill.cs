﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Formations;

namespace Loderpit.Skills
{
    public class ThrowRopeSkill : Skill
    {
        private float _range;

        public float range { get { return _range; } }

        public ThrowRopeSkill(int level, float range)
            : base(SkillType.ThrowRope, level, true)
        {
            _range = range;
        }
    }

    public class ExecuteThrowRopeSkill : ExecuteSkill
    {
        private Vector2 _anchor;
        private Formation _formationToRemove;

        public Vector2 anchor { get { return _anchor; } }
        public Formation formationToRemove { get { return _formationToRemove; } set { _formationToRemove = value; } }

        public ExecuteThrowRopeSkill(Skill skill, Vector2 anchor)
            : base(skill)
        {
            _anchor = anchor;
            _delay = 180;
        }
    }
}
