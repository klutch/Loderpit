using System;
using SFML.Window;
using SFML.Graphics;
using Loderpit.Systems;

namespace Loderpit
{
    public class Particle
    {
        private RectangleShape _shape;
        private Vector2f _velocity;
        private Vector2f _acceleration;
        private int _timeToLive;
        private float _rotationVelocity;

        public RectangleShape shape { get { return _shape; } }
        public int timeToLive { get { return _timeToLive; } set { _timeToLive = value; } }
        public Vector2f position { get { return _shape.Position; } set { _shape.Position = value; } }
        public Vector2f velocity { get { return _velocity; } set { _velocity = value; } }
        public Vector2f acceleration { get { return _acceleration; } set { _acceleration = value; } }
        public float rotation { get { return _shape.Rotation; } set { _shape.Rotation = value; } }
        public float rotationVelocity { get { return _rotationVelocity; } set { _rotationVelocity = value; } }
        public Color color { get { return _shape.FillColor; } set { _shape.FillColor = value; } }
        public Texture texture
        {
            set
            {
                _shape.Texture = value;
            }
        }

        public Particle()
        {
            _shape = new RectangleShape();
        }
    }
}
