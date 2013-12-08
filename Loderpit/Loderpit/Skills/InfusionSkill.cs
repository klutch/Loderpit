using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public class InfusionSkill : Skill
    {
        public int maxHpMod { get { return calculateMaxHpMod(); } }
        public int strengthMod { get { return calculateStrengthMod(); } }
        public int armorClassMod { get { return calculateArmorClassMod(); } }
        public int timeToLive { get { return 60 * 30; } }

        public InfusionSkill(int entityId, int level)
            : base(SkillType.Infusion, entityId, level, true)
        {
            _baseCooldown = 360;
            _range = 6f;
        }

        private int calculateMaxHpMod()
        {
            switch (_level)
            {
                default: return 6;
            }
        }

        private int calculateStrengthMod()
        {
            switch (_level)
            {
                default: return 2;
            }
        }

        private int calculateArmorClassMod()
        {
            switch (_level)
            {
                default: return 2;
            }
        }
    }

    public class ExecuteInfusionSkill : ExecuteSkill
    {
        private int _targetEntityId;

        public int targetEntityId { get { return _targetEntityId; } }

        public ExecuteInfusionSkill(Skill skill, int targetEntityId, Func<bool> isDelayConditionMetCallback)
            : base(skill, isDelayConditionMetCallback)
        {
            _targetEntityId = targetEntityId;
            _delay = 60;
        }
    }
}
