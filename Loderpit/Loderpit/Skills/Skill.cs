using System;
using System.Collections.Generic;

namespace Loderpit.Skills
{
    public enum SkillType
    {
        MeleeAttack,
        RangedAttack,
        ThrowRope,
        BuildBridge
    }

    abstract public class Skill
    {
        protected SkillType _type;
        protected int _level;
        protected int _cooldown;
        protected bool _activatable;

        public SkillType type { get { return _type; } }
        public int level { get { return _level; } set { _level = value; } }
        public int cooldown { get { return _cooldown; } set { _cooldown = value; } }
        public bool activatable { get { return _activatable; } }

        public Skill(SkillType type, int level, bool activatable)
        {
            _type = type;
            _level = level;
            _activatable = activatable;
        }
    }
}
