using System;
using System.Diagnostics;
using System.Collections.Generic;

namespace Loderpit.Formations
{
    public class SplitFormation : Formation
    {
        private float _gapDistance;
        private int _splitAfterSlot;

        public SplitFormation(List<int> groupEntities, float position, int splitAfterSlot, float gapDistance = 4f)
            : base(FormationType.Split, 10, groupEntities, position, 0)
        {
            _splitAfterSlot = splitAfterSlot;
            _gapDistance = gapDistance;
            _maxSpeed = 0;
        }

        public override float getSlotPosition(int slot)
        {
            int slotDifference = slot - _splitAfterSlot;
            bool onLeft = slotDifference < 0;

            return _position + slotDifference * _idealSpacing + _gapDistance * (onLeft ? 0f : 0.5f);
        }
    }
}
