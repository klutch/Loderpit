using System;
using System.Collections.Generic;

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
        ProximityMine,
        Fortification,

        // Archer
        ShieldOfThorns,
        PowerShot,
        Deadeye,
        ArrowTime,
        Volley,
        Piercing,

        // Fighter
        Kick,
        PowerSwing,
        Bloodletter,
        Fatality,
        BattleCry,
        Frenzy,

        // Defender
        Block,
        ShieldBash,
        SpikedShield,
        Guardian,
        Riposte,
        GolemStance,

        // Mage
        Ignite,
        Fireball,
        FlameAura,
        RainOfFire,
        GaleForce,
        Explosivity,

        // Healer
        Heal,
        HealingBlast,
        Infusion,
        Dispel,
        Regeneration
    }

    abstract public class Skill
    {
        protected SkillType _type;
        protected int _entityId;
        protected int _level;
        protected int _cooldown;
        protected int _lastMaxCooldown;
        protected int _baseCooldown;    // not used by all skills (i.e.,  melee/ranged attacks)
        protected bool _activatable;
        protected float _range;

        public SkillType type { get { return _type; } }
        public int level { get { return _level; } set { _level = value; } }
        public int cooldown { get { return _cooldown; } }
        public bool activatable { get { return _activatable; } }
        public float range { get { return _range; } }
        public float cooldownPercentage { get { return (float)_cooldown / (float)_lastMaxCooldown; } }

        public Skill(SkillType type, int entityId, int level, bool activatable)
        {
            _type = type;
            _entityId = entityId;
            _level = level;
            _activatable = activatable;
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
