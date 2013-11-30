using System;
using System.Collections.Generic;

namespace Loderpit.Components
{
    public enum ExternalMovementSpeedType
    {
        ShieldBlock
    }

    public class ExternalMovementSpeedsComponent : IComponent
    {
        private int _entityId;
        private Dictionary<ExternalMovementSpeedType, int> _speeds;

        public int entityId { get { return _entityId; } set { _entityId = value; } }
        public ComponentType componentType { get { return ComponentType.ExternalMovementSpeeds; } }

        public ExternalMovementSpeedsComponent(int entityId)
        {
            _speeds = new Dictionary<ExternalMovementSpeedType, int>();
        }

        public void addExternalMovementSpeed(ExternalMovementSpeedType type, int speed)
        {
            if (!_speeds.ContainsKey(type))
            {
                _speeds.Add(type, speed);
            }
            else
            {
                _speeds[type] = speed;
            }
        }

        public void removeExternalMovementSpeed(ExternalMovementSpeedType type)
        {
            _speeds.Remove(type);
        }

        public bool tryGetExternalMovementSpeed(ExternalMovementSpeedType type, out int speed)
        {
            speed = 0;

            if (_speeds.ContainsKey(type))
            {
                speed = _speeds[type];
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
