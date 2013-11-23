using System;
using System.Collections.Generic;
using SFML.Graphics;
using Loderpit.Managers;

namespace Loderpit.Screens
{
    public enum ScreenType
    {
        CreateTeam,
        Level,
        InterLevel
    };

    abstract public class Screen
    {
        protected ScreenType _type;
        private List<ScreenComponent> _screenComponents;
        private List<ScreenComponent> _componentsToRemove;

        public ScreenType type { get { return _type; } }
        public List<ScreenComponent> screenComponents { get { return _screenComponents; } }

        public Screen(ScreenType type)
        {
            _type = type;
            _screenComponents = new List<ScreenComponent>();
            _componentsToRemove = new List<ScreenComponent>();

            loadContent();
            initialize();
        }

        abstract public void loadContent();
        abstract public void initialize();

        virtual public void addScreenComponent(ScreenComponent screenComponent)
        {
            _screenComponents.Add(screenComponent);
        }

        virtual public void removeScreenComponent(ScreenComponent screenComponent)
        {
            _componentsToRemove.Add(screenComponent);
        }

        virtual public void update()
        {
            // Remove screen components
            foreach (ScreenComponent screenComponent in _componentsToRemove)
            {
                _screenComponents.Remove(screenComponent);
            }
            _componentsToRemove.Clear();

            // Update screen components
            foreach (ScreenComponent screenComponent in _screenComponents)
            {
                screenComponent.update();
            }
        }

        virtual public void draw()
        {
            foreach (ScreenComponent screenComponent in _screenComponents)
            {
                screenComponent.draw();
            }
        }
    }
}
