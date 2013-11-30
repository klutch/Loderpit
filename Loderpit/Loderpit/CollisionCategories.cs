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
        Characters = 1,
        CharacterFeet = 2,
        Terrain = 4
    }
}
