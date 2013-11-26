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
        Character = 1,
        Rope = 2,
        Bridge = 4,
        Ground = 8,
        CharacterInteractionSensor = 16,
        CharacterInteractionReceptor = 32,
        Ceiling = 64
    }
}
