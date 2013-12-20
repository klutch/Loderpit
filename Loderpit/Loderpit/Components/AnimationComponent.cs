using System;
using System.Collections.Generic;
using SFML.Graphics;

namespace Loderpit.Components
{
    public class AnimationComponent : IComponent
    {
        private AnimationType _animationType;
        private AnimationCategory _animationCategory;
        private int _entityId;
        private int _ticksSinceFrameChange;
        private int _ticksPerFrame;
        private int _frameIndex;
        private RectangleShape _shape;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Animation; } }
        public int ticksSinceFrameChange { get { return _ticksSinceFrameChange; } set { _ticksSinceFrameChange = value; } }
        public int ticksPerFrame { get { return _ticksPerFrame; } set { _ticksPerFrame = value; } }
        public int frameIndex { get { return _frameIndex; } set { _frameIndex = value; } }
        public AnimationType animationType { get { return _animationType; } set { _animationType = value; } }
        public AnimationCategory animationCategory { get { return _animationCategory; } }
        public RectangleShape shape { get { return _shape; } }

        public AnimationComponent(int entityId, AnimationCategory animationCategory, AnimationType animationType, int ticksPerFrame)
        {
            _entityId = entityId;
            _animationCategory = animationCategory;
            _animationType = animationType;
            _ticksPerFrame = ticksPerFrame;
            _ticksSinceFrameChange = _ticksPerFrame;
            _shape = new RectangleShape();
        }
    }
}
