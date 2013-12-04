using System;
using System.Collections.Generic;

namespace Loderpit.SpellEffects
{
    public enum SpellEffectType
    {
        DamageShield,
        KnockbackProc,
        IgniteProc,
        Ignite,
        StatBuff,
        DamageAura
    }

    abstract public class SpellEffect
    {
        protected int _sourceEntityId;
        protected SpellEffectType _type;
        protected bool _affectsSelf;
        protected bool _affectsFriendly;
        protected bool _affectsNeutral;
        protected bool _affectsHostile;
        protected int _timeToLive;      // -1 == infinite, 0 == dead, > 0 == alive
        protected Action<int, int> _onHitByOther;
        protected Action<int, int> _onHitOther;

        public SpellEffectType type { get { return _type; } }
        public bool affectsSelf { get { return _affectsSelf; } }
        public bool affectsFriendly { get { return _affectsFriendly; } }
        public bool affectsNeutral { get { return _affectsNeutral; } }
        public bool affectsHostile { get { return _affectsHostile; } }
        public int timeToLive { get { return _timeToLive; } set { _timeToLive = value; } }
        public int sourceEntityId { get { return _sourceEntityId; } }

        public SpellEffect(SpellEffectType type, int sourceEntityId)
        {
            _type = type;
            _sourceEntityId = sourceEntityId;
            _timeToLive = -1;
        }

        public void onHitByOther(int attackerId, int defenderId)
        {
            if (_onHitByOther != null)
            {
                _onHitByOther(attackerId, defenderId);
            }
        }

        public void onHitOther(int attackerId, int defenderId)
        {
            if (_onHitOther != null)
            {
                _onHitOther(attackerId, defenderId);
            }
        }
    }
}
