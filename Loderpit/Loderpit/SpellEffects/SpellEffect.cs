using System;
using System.Collections.Generic;

namespace Loderpit.SpellEffects
{
    public enum SpellEffectType
    {
    }

    abstract public class SpellEffect
    {
        private SpellEffectType _type;
        private bool _affectsSelf;
        private bool _affectsFriendly;
        private bool _affectsNeutral;
        private bool _affectsHostile;
        private int _timeToLive;   // -1 == infinite, 0 == dead, > 0 == alive

        public SpellEffectType type { get { return _type; } }
        public bool affectsSelf { get { return _affectsSelf; } }
        public bool affectsFriendly { get { return _affectsFriendly; } }
        public bool affectsNeutral { get { return _affectsNeutral; } }
        public bool affectsHostile { get { return _affectsHostile; } }

        public SpellEffect(SpellEffectType type, bool affectsSelf, bool affectsFriendly, bool affectsNeutral, bool affectsHostile, int timeToLive)
        {
            _type = type;
            _affectsSelf = affectsSelf;
            _affectsFriendly = affectsFriendly;
            _affectsHostile = affectsHostile;
            _timeToLive = timeToLive;
        }
    }
}
