using System;
using SFML.Window;
using SFML.Graphics;
using Loderpit.Systems;

namespace Loderpit
{
    public class Particle : Drawable
    {
        private RectangleShape _shape;
        private Vector2f _position;
        private Vector2f _velocity;
        private Vector2f _acceleration;
        private int _timeToLive;
        private float _rotationVelocity;
        private Transform _transform;
        private bool _recalcTransform;
        private float _scale;

        public RectangleShape shape { get { return _shape; } }
        public int timeToLive { get { return _timeToLive; } set { _timeToLive = value; } }
        public Vector2f velocity { get { return _velocity; } set { _velocity = value; } }
        public Vector2f acceleration { get { return _acceleration; } set { _acceleration = value; } }
        public float rotation { get { return _shape.Rotation; } set { _shape.Rotation = value; } }
        public float rotationVelocity { get { return _rotationVelocity; } set { _rotationVelocity = value; } }
        public Color color { get { return _shape.FillColor; } set { _shape.FillColor = value; } }
        public float scale
        {
            get { return _scale; }
            set
            {
                _recalcTransform = true;
                _scale = value;
            }
        }
        public Vector2f position
        {
            get
            {
                return _position;
            }
            set
            {
                _recalcTransform = true;
                _position = value;
            }
        }
        public Texture texture
        {
            set
            {
                _shape.Texture = value;
                _shape.Size = new Vector2f(value.Size.X, value.Size.Y) / CameraSystem.ORIGINAL_SCALE;
                _shape.Origin = _shape.Size * 0.5f;
            }
        }

        public Particle()
        {
            _shape = new RectangleShape();
            _transform = Transform.Identity;
        }

        public void Draw(RenderTarget renderTarget, RenderStates renderStates)
        {
            if (_recalcTransform)
            {
                _transform = Transform.Identity;
                _transform.Translate(_position);
                _transform.Scale(_scale, _scale);
                _recalcTransform = false;
            }

            renderStates.Transform *= _transform;
            renderTarget.Draw(_shape, renderStates);
        }
    }
}
