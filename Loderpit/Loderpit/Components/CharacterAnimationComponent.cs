using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Loderpit.Components
{
    public class CharacterAnimationComponent : IComponent
    {
        private CharacterAnimationType _type;
        private int _entityId;
        private int _ticksSinceFrameChange;
        private int _ticksPerFrame;
        private int _frameIndex;
        private RectangleShape _shape;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.CharacterAnimation; } }
        public int ticksSinceFrameChange { get { return _ticksSinceFrameChange; } set { _ticksSinceFrameChange = value; } }
        public int ticksPerFrame { get { return _ticksPerFrame; } set { _ticksPerFrame = value; } }
        public int frameIndex { get { return _frameIndex; } set { _frameIndex = value; } }
        public CharacterAnimationType type { get { return _type; } set { _type = value; } }
        public RectangleShape shape { get { return _shape; } }
        
        public CharacterAnimationComponent(int entityId, int ticksPerFrame)
        {
            _entityId = entityId;
            _ticksPerFrame = ticksPerFrame;
            _shape = new RectangleShape();
        }
    }
}
