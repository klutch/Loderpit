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
        ShieldOfThorns,
        PowerShot
    }

    abstract public class Skill
    {
        protected SkillType _type;
        protected int _level;
        protected int _cooldown;
        protected int _lastMaxCooldown;
        protected int _baseCooldown;    // not used by all skills (i.e.,  melee/ranged attacks)
        protected bool _activatable;
        protected List<SpellEffect> _passiveSpellEffects;

        public SkillType type { get { return _type; } }
        public int level { get { return _level; } set { _level = value; } }
        public int cooldown { get { return _cooldown; } }
        public int baseCooldown { get { return _baseCooldown; } }
        public bool activatable { get { return _activatable; } }
        public List<SpellEffect> passiveSpellEffects { get { return _passiveSpellEffects; } }
        public float cooldownPercentage { get { return (float)_cooldown / (float)_lastMaxCooldown; } }

        public Skill(SkillType type, int level, bool activatable)
        {
            _type = type;
            _level = level;
            _activatable = activatable;
            _passiveSpellEffects = new List<SpellEffect>();
        }

        public void setCooldown(int value)
        {
            _cooldown = value;
            _lastMaxCooldown = value;
        }

        public void decrementCooldown()
        {
            _cooldown--;
        }
    }
}
