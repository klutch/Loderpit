using System;
using System.Collections.Generic;

namespace Loderpit.Screens
{
    public class MainMenuScreen : Screen
    {
        public ScreenType screenType { get { return ScreenType.MainMenu; } }

        public MainMenuScreen()
            : base(ScreenType.MainMenu)
        {
        }

        override public void initialize()
        {
        }

        override public void loadContent()
        {
        }

        public override void update()
        {
            base.update();
        }

        public override void draw()
        {
            base.draw();
        }
    }
}
