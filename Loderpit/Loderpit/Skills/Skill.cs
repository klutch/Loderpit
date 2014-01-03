using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

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
        ServoBot,
        BattleDrone,

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
        protected bool _isUpgradable;

        public SkillType type { get { return _type; } }
        public int entityId { get { return _entityId; } }
        public int level { get { return _level; } set { _level = value; } }
        public int cooldown { get { return _cooldown; } }
        public bool activatable { get { return _activatable; } }
        public float range { get { return _range; } }
        public float cooldownPercentage { get { return (float)_cooldown / (float)_lastMaxCooldown; } }
        public bool isUpgradable { get { return _isUpgradable; } }

        public Skill(SkillType type, int entityId, int level, bool activatable, bool isUpgradable = true)
        {
            _type = type;
            _entityId = entityId;
            _level = level;
            _activatable = activatable;
            _isUpgradable = isUpgradable;
        }

        public static Skill create(SkillType skillType, int entityId, int level)
        {
            switch (skillType)
            {
                case SkillType.ArrowTime:
                    return new ArrowTimeSkill(entityId, level);

                case SkillType.BattleCry:
                    return new BattleCrySkill(entityId, level);

                case SkillType.BattleDrone:
                    return new BattleDroneSkill(entityId, level);

                case SkillType.Block:
                    return new BlockSkill(entityId, level);

                case SkillType.Bloodletter:
                    return new BloodletterSkill(entityId, level);

                case SkillType.BuildBridge:
                    return new BuildBridgeSkill(entityId, level);

                case SkillType.Deadeye:
                    return new DeadeyeSkill(entityId, level);

                case SkillType.Dispel:
                    return new DispelSkill(entityId, level);

                case SkillType.Explosivity:
                    return new ExplosivitySkill(entityId, level);

                case SkillType.Fatality:
                    return new FatalitySkill(entityId, level);

                case SkillType.Fireball:
                    return new FireballSkill(entityId, level);

                case SkillType.FlameAura:
                    return new FlameAuraSkill(entityId, level);

                case SkillType.Fortification:
                    return new FortificationSkill(entityId, level);

                case SkillType.Frenzy:
                    return new FrenzySkill(entityId, level);

                case SkillType.GaleForce:
                    return new GaleForceSkill(entityId, level, new Vector2(1.5f, -1f));

                case SkillType.GolemStance:
                    return new GolemStanceSkill(entityId, level);

                case SkillType.Guardian:
                    return new GuardianSkill(entityId, level);

                case SkillType.Heal:
                    return new HealingBlastSkill(entityId, level);

                case SkillType.HealingBlast:
                    return new HealingBlastSkill(entityId, level);

                case SkillType.Ignite:
                    return new IgniteSkill(entityId, level);

                case SkillType.Infusion:
                    return new InfusionSkill(entityId, level);

                case SkillType.Kick:
                    return new KickSkill(entityId, level);

                case SkillType.Piercing:
                    return new PiercingSkill(entityId, level);

                case SkillType.PowerShot:
                    return new PowerShotSkill(entityId, level);

                case SkillType.PowerSwing:
                    return new PowerSwingSkill(entityId, level);

                case SkillType.ProximityMine:
                    return new ProximityMineSkill(entityId, level);

                case SkillType.RainOfFire:
                    return new RainOfFireSkill(entityId, level);

                case SkillType.Regeneration:
                    return new RegenerationSkill(entityId, level);

                case SkillType.Riposte:
                    return new RiposteSkill(entityId, level);

                case SkillType.ServoBot:
                    return new ServoBotSkill(entityId, level);

                case SkillType.ShieldBash:
                    return new ServoBotSkill(entityId, level);

                case SkillType.ShieldOfThorns:
                    return new ShieldOfThornsSkill(entityId, level);

                case SkillType.SpikedShield:
                    return new SpikedShieldSkill(entityId, level);

                case SkillType.ThrowRope:
                    return new ThrowRopeSkill(entityId, level);

                case SkillType.Volley:
                    return new VolleySkill(entityId, level);
            }

            System.Diagnostics.Debug.Assert(false, String.Format("No construction method for: {0}", skillType));
            return null;
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
