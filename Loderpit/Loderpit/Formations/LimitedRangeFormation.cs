using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loderpit.Formations
{
    public class LimitedRangeFormation : Formation
    {
        private float _minPosition;
        private float _maxPosition;

        public float minPosition { get { return _minPosition; } }
        public float maxPosition { get { return _maxPosition; } }

        public LimitedRangeFormation(List<int> groupEntities, float position, int speed, float minPosition, float maxPosition)
            : base(FormationType.LimitedRange, 5, groupEntities, position, speed)
        {
            _minPosition = minPosition;
            _maxPosition = maxPosition;
        }
    }
}
