using System;
using System.Collections.Generic;
using System.Linq;
using Loderpit.Screens;

namespace Loderpit.Managers
{
    public class ScreenManager
    {
        private static Dictionary<ScreenType, Screen> _screens = new Dictionary<ScreenType, Screen>();

        public static LevelScreen levelScreen { get { return (LevelScreen)_screens.First(pair => pair.Key == ScreenType.Level).Value; } }

        public static void addScreen(Screen screen)
        {
            _screens.Add(screen.type, screen);
        }

        public static void update()
        {
            foreach (Screen screen in _screens.Values)
            {
                screen.update();
            }
        }

        public static void draw()
        {
            foreach (Screen screen in _screens.Values)
            {
                screen.draw();
            }
        }
    }
}
