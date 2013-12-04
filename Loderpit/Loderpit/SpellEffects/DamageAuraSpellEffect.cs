using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class DamageAuraSpellEffect : SpellEffect, IAoESpellEffect, IDoTSpellEffect
    {
        private string _damageDie;
        private float _radius;
        private int _baseDelay;
        private int _currentDelay;

        public string damageDie { get { return _damageDie; } }
        public float radius { get { return _radius; } }
        public int baseDelay { get { return _baseDelay; } }
        public int currentDelay { get { return _currentDelay; } set { _currentDelay = value; } }

        public DamageAuraSpellEffect(int sourceEntityId, string damageDie, float radius, int baseDelay, int tickCount, bool affectsSelf, bool affectsFriendly, bool affectsHostile, bool affectsNeutral)
            : base(SpellEffectType.DamageAura, sourceEntityId)
        {
            _damageDie = damageDie;
            _radius = radius;
            _baseDelay = baseDelay;
            _affectsSelf = affectsSelf;
            _affectsFriendly = affectsFriendly;
            _affectsHostile = affectsHostile;
            _affectsNeutral = affectsNeutral;
        }

        public void onTick(int ownerId, int receiverId)
        {
            SystemManager.combatSystem.applySpellDamage(receiverId, Roller.roll(_damageDie));
        }
    }
}
