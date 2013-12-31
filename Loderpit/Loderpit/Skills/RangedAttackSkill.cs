using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class RangedAttackSkill : Skill
    {
        private string _textureResourceId;
        private DamageType _damageType;

        public string textureResourceId { get { return _textureResourceId; } }
        public DamageType damageType { get { return _damageType; } }

        public RangedAttackSkill(int entityId, int level, DamageType damageType, string textureResourceId = "ranged_attack_skill_icon")
            : base(SkillType.RangedAttack, entityId, level, true, false)
        {
            _range = 8f;
            _damageType = damageType;
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
