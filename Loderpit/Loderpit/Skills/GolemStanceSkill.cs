using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class GolemStanceSkill : Skill
    {
        public float damageMitigationPercentage { get { return 0.5f; } }

        public GolemStanceSkill(int entityId, int level)
            : base(SkillType.GolemStance, entityId, level, true)
        {
            _baseCooldown = 360;
        }
    }

    public class ExecuteGolemStanceSkill : ExecuteSkill
    {
        private Vector2 _position;

        public ExecuteGolemStanceSkill(Skill skill, Vector2 position, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _position = position;
        }
    }
}
