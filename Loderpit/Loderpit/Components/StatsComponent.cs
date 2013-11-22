using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public class StatsComponent : IComponent
    {
        private int _entityId;
        private int _baseHp;
        private int _currentHp;
        private int _dexterity;
        private int _strength;
        private int _intelligence;
        private int _attackDelay;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Stats; } }
        public int baseHp { get { return _baseHp; } set { _baseHp = value; } }
        public int currentHp { get { return _currentHp; } set { _currentHp = Math.Max(value, 0); } }
        public int dexterity { get { return _dexterity; } set { _dexterity = value; } }
        public int strength { get { return _strength; } set { _strength = value; } }
        public int intelligence { get { return _intelligence; } set { _intelligence = value; } }
        public int attackDelay { get { return _attackDelay; } set { _attackDelay = value; } }

        public StatsComponent(int entityId, int baseHp, int currentHp, int dexterity, int strength, int intelligence, int attackDelay)
        {
            _entityId = entityId;
            _baseHp = baseHp;
            _currentHp = currentHp;
            _dexterity = dexterity;
            _strength = strength;
            _intelligence = intelligence;
            _attackDelay = attackDelay;
        }
    }
}
