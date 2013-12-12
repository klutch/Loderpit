using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Loderpit.Skills
{
    public class FortificationSkill : Skill
    {
        public int maxHp { get { return calculateMaxHp(); } }

        public FortificationSkill(int entityId, int level)
            : base(SkillType.Fortification, entityId, level, true)
        {
            _range = 2f;
            _baseCooldown = 1080;
        }

        private int calculateMaxHp()
        {
            switch (_level)
            {
                default: return 20;
            }
        }
    }

    public class ExecuteFortificationSkill : ExecuteSkill
    {
        private Vector2 _target;

        public Vector2 target { get { return _target; } }

        public ExecuteFortificationSkill(Skill skill, Vector2 target, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _target = target;
            _delay = 180;
        }
    }
}
