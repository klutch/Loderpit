using System;
using SFML.Window;

namespace Loderpit.Particles
{
    interface IAcceleration
    {
        Vector2f acceleration { get; set; }
    }
}
