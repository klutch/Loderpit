using System;
using SFML.Window;
using SFML.Graphics;

namespace Loderpit.Components
{
    public class BloodColorComponent : IComponent
    {
        private int _entityId;
        private Color _color;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.BloodColor; } }
        public Color color { get { return _color; } }

        public BloodColorComponent(int entityId, Color color)
        {
            _color = color;
        }
    }
}
