using System;
using System.Collections.Generic;

namespace Loderpit.Screens
{
    abstract public class ScreenComponent
    {
        protected Screen _screen;

        public ScreenComponent(Screen screen)
        {
            _screen = screen;
        }

        abstract public void update();
        abstract public void draw();
    }
}
