using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Loderpit.Particles
{
    interface IFadeAlpha
    {
        float alpha { get; set; }
        float changeAlphaPerFrame { get; set; }
    }
}
