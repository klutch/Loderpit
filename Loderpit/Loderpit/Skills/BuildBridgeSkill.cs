using System;
using System.Collections.Generic;

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
}
