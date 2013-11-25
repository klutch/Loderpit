using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class RangedAttackSkill : Skill
    {
        private string _textureResourceId;
        private float _range;

        public string textureResourceId { get { return _textureResourceId; } }
        public float range { get { return _range; } }

        public RangedAttackSkill(int level, float range, string textureResourceId = "ranged_attack_skill_icon")
            : base(SkillType.RangedAttack, level, true)
        {
            _range = range;
            _textureResourceId = textureResourceId;
        }
    }

    public class ExecuteRangedAttackSkill : ExecuteSkill
    {
        private int _defenderId;

        public ExecuteRangedAttackSkill(Skill skill, int defenderId)
            : base(skill)
        {
            _defenderId = defenderId;
        }
    }
}
