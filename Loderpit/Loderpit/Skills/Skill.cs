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
        PowerShot,

        // Fighter
        Kick,
        PowerSwing,

        // Defender
        Block,
        ShieldBash,

        // Mage
        Ignite,
        Fireball,

        // Healer
        Heal,
        HealingBlast
    }

    abstract public class Skill
    {
        protected SkillType _type;
        protected int _level;
        protected int _cooldown;
        protected int _lastMaxCooldown;
        protected int _baseCooldown;    // not used by all skills (i.e.,  melee/ranged attacks)
        protected bool _activatable;
        protected float _range;
        protected List<SpellEffect> _passiveSpellEffects;
        protected List<SpellEffect> _onActivateSpellEffects;

        public SkillType type { get { return _type; } }
        public int level { get { return _level; } set { _level = value; } }
        public int cooldown { get { return _cooldown; } }
        public bool activatable { get { return _activatable; } }
        public float range { get { return _range; } }
        public List<SpellEffect> passiveSpellEffects { get { return _passiveSpellEffects; } }
        public List<SpellEffect> onActivateSpellEffects { get { return _onActivateSpellEffects; } }
        public float cooldownPercentage { get { return (float)_cooldown / (float)_lastMaxCooldown; } }

        public Skill(SkillType type, int level, bool activatable)
        {
            _type = type;
            _level = level;
            _activatable = activatable;
            _passiveSpellEffects = new List<SpellEffect>();
            _onActivateSpellEffects = new List<SpellEffect>();
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

        virtual public int calculateBaseCooldown()
        {
            return _baseCooldown;
        }
    }

    abstract public class ExecuteSkill
    {
        protected Skill _skill;
        protected int _delay;
        protected Func<bool> _isDelayConditionMetCallback;

        public Skill skill { get { return _skill; } }
        public int delay { get { return _delay; } set { _delay = value; } }

        public ExecuteSkill(Skill skill, Func<bool> isDelayConditionMetCallback = null)
        {
            _skill = skill;
            _isDelayConditionMetCallback = isDelayConditionMetCallback;
        }

        public bool isDelayConditionMet()
        {
            if (_isDelayConditionMetCallback == null)
            {
                return true;
            }
            else
            {
                return _isDelayConditionMetCallback();
            }
        }
    }
}
