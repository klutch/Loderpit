﻿using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.SpellEffects
{
    public class IgniteSpellEffect : SpellEffect, IDoTSpellEffect
    {
        private string _damageDie;
        private int _baseDelay;
        private int _currentDelay;

        public string damageDie { get { return _damageDie; } }
        public int baseDelay { get { return _baseDelay; } }
        public int currentDelay { get { return _currentDelay; } set { _currentDelay = value; } }

        public IgniteSpellEffect(int sourceEntityId, string damageDie, int baseDelay, int tickCount)
            : base(SpellEffectType.Ignite, sourceEntityId)
        {
            _damageDie = damageDie;
            _baseDelay = baseDelay;
            _timeToLive = baseDelay * tickCount;
            _affectsSelf = true;
        }

        public void onTick(int ownerId, int receiverId)
        {
            SystemManager.combatSystem.applySpellDamage(receiverId, Roller.roll(_damageDie));
        }
    }
}