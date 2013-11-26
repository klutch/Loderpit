using System;
using System.Collections.Generic;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;

namespace Loderpit
{
    public enum CollisionCategory
    {
        None = 0,
        All = 0xFFFF,
        PlayerCharacters = 1,
        EnemyCharacters = 2
    }
}
