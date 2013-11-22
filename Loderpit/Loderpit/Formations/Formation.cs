using System;
using System.Collections.Generic;

namespace Loderpit.Formations
{
    public enum FormationType
    {
        Default,
        LimitedRange,
        Split
    }

    abstract public class Formation
    {
        protected FormationType _type;
        protected int _priority;                // highest processed first
        protected List<int> _groupEntities;     // SHOULD NEVER BE MODIFIED
        protected float _position;
        protected int _speed;
        protected int _storedSpeed;
        protected int _maxSpeed = 4;
        protected float _idealSpacing = 2f;

        public FormationType type { get { return _type; } }
        public float position { get { return _position; } set { _position = value; } }
        public int speed { get { return _speed; } set { _speed = Math.Max(-_maxSpeed, Math.Min(value, _maxSpeed)); } }
        public int storedSpeed { get { return _storedSpeed; } set { _storedSpeed = value; } }
        public int priority { get { return _priority; } }
        public int maxSpeed { get { return _maxSpeed; } }

        public Formation(FormationType type, int priority, List<int> groupEntities, float position, int speed)
        {
            _type = type;
            _priority = priority;
            _groupEntities = groupEntities;
            _position = position;
            _speed = speed;
            _storedSpeed = _speed;
        }

        virtual public float getSlotPosition(int slot)
        {
            float formationIdealWidth = _idealSpacing * (_groupEntities.Count - 1);

            return _position - (formationIdealWidth * 0.5f) + slot * _idealSpacing;
        }

        public float getSlotPositionByEntityId(int entityId)
        {
            int slot = _groupEntities.IndexOf(entityId);

            return getSlotPosition(slot);
        }
    }
}
