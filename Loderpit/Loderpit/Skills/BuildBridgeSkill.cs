using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Formations;

namespace Loderpit.Skills
{
    public class BuildBridgeSkill : Skill
    {
        private float _range;

        public float range { get { return _range; } }

        public BuildBridgeSkill(int level, float range)
            : base(SkillType.BuildBridge, level, true)
        {
            _range = range;
        }
    }

    public class ExecuteBuildBridgeSkill : ExecuteSkill
    {
        private Vector2 _anchorA;
        private Vector2 _anchorB;
        private Formation _formationToRemove;

        public Vector2 anchorA { get { return _anchorA; } }
        public Vector2 anchorB { get { return _anchorB; } }
        public Formation formationToRemove { get { return _formationToRemove; } set { _formationToRemove = value; } }

        public ExecuteBuildBridgeSkill(Skill skill, Vector2 anchorA, Vector2 anchorB)
            : base (skill)
        {
            _anchorA = anchorA;
            _anchorB = anchorB;
            _delay = 180;
        }
    }
}
