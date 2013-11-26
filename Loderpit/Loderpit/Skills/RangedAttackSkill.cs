using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class RangedAttackSkill : Skill
    {
        private string _textureResourceId;

        public string textureResourceId { get { return _textureResourceId; } }

        public RangedAttackSkill(int level, string textureResourceId = "ranged_attack_skill_icon")
            : base(SkillType.RangedAttack, level, true)
        {
            _range = 8f;
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
