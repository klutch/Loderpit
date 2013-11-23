using System;
using System.Collections.Generic;
using System.Linq;
using Loderpit.Screens;

namespace Loderpit.Managers
{
    public class ScreenManager
    {
        private static Dictionary<ScreenType, Screen> _screens = new Dictionary<ScreenType, Screen>();
        private static List<ScreenType> _screensToRemove = new List<ScreenType>();
        private static List<Screen> _screensToAdd = new List<Screen>();

        public static LevelScreen levelScreen { get { return (LevelScreen)_screens.First(pair => pair.Key == ScreenType.Level).Value; } }

        public static void addScreen(Screen screen)
        {
            _screensToAdd.Add(screen);
        }

        public static void removeScreen(ScreenType screenType)
        {
            _screensToRemove.Add(screenType);
        }

        public static void update()
        {
            // Remove screens
            foreach (ScreenType screenType in _screensToRemove)
            {
                _screens.Remove(screenType);
            }
            _screensToRemove.Clear();

            // Add screens
            foreach (Screen screen in _screensToAdd)
            {
                _screens.Add(screen.type, screen);
            }
            _screensToAdd.Clear();

            // Update screens
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
