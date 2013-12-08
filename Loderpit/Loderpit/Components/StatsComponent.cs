using System;
using System.Collections.Generic;
using Loderpit.Managers;

namespace Loderpit.Components
{
    public class StatsComponent : IComponent
    {
        private int _entityId;
        private int _baseHp;
        private int _currentHp;
        private int _baseDexterity;
        private int _baseStrength;
        private int _baseIntelligence;
        private int _attackDelay;
        private string _unarmedHitDie;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Stats; } }
        public int baseHp { get { return _baseHp; } set { _baseHp = value; _currentHp = value; } }
        public int currentHp { get { return _currentHp; } set { _currentHp = Math.Max(Math.Min(value, SystemManager.statSystem.getMaxHp(_entityId)), 0); } }
        public int baseDexterity { get { return _baseDexterity; } set { _baseDexterity = value; } }
        public int baseStrength { get { return _baseStrength; } set { _baseStrength = value; } }
        public int baseIntelligence { get { return _baseIntelligence; } set { _baseIntelligence = value; } }
        public int attackDelay { get { return _attackDelay; } set { _attackDelay = value; } }
        public string unarmedHitDie { get { return _unarmedHitDie; } set { _unarmedHitDie = value; } }

        public StatsComponent(int entityId, int baseHp, int currentHp, int baseDexterity, int baseStrength, int baseIntelligence, int attackDelay, string unarmedHitDie = "d6")
        {
            _entityId = entityId;
            _baseHp = baseHp;
            _currentHp = currentHp;
            _baseDexterity = baseDexterity;
            _baseStrength = baseStrength;
            _baseIntelligence = baseIntelligence;
            _attackDelay = attackDelay;
            _unarmedHitDie = unarmedHitDie;
        }
    }
}
