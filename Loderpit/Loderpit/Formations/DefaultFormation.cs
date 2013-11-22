using System;
using System.Collections.Generic;

namespace Loderpit.Formations
{
    public class DefaultFormation : Formation
    {
        public DefaultFormation(List<int> groupEntities, float position, int speed)
            : base(FormationType.Default, 0, groupEntities, position, speed)
        {
        }
    }
}
