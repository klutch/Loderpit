using System;
using System.Collections.Generic;
using Loderpit.SpellEffects;

namespace Loderpit.Skills
{
    public enum SkillType
    {
        // Common
        MeleeAttack,
        RangedAttack,

        // Engineer
        ThrowRope,
        BuildBridge,

        // Archer
        ShieldOfThorns
    }

    abstract public class Skill
    {
        protected SkillType _type;
        protected int _level;
        protected int _cooldown;
        protected bool _activatable;
        protected List<SpellEffect> _passiveSpellEffects;

        public SkillType type { get { return _type; } }
        public int level { get { return _level; } set { _level = value; } }
        public int cooldown { get { return _cooldown; } set { _cooldown = value; } }
        public bool activatable { get { return _activatable; } }
        public List<SpellEffect> passiveSpellEffects { get { return _passiveSpellEffects; } }

        public Skill(SkillType type, int level, bool activatable)
        {
            _type = type;
            _level = level;
            _activatable = activatable;
            _passiveSpellEffects = new List<SpellEffect>();
        }
    }
}
