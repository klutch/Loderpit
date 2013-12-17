using System;
using System.Collections.Generic;
using SFML.Graphics;
using SFML.Window;

namespace Loderpit.Particles
{
    public class BloodParticle : Particle, IFadeAlpha, IAcceleration, IVelocity, ITimeToLive, IPosition
    {
        private float _changeAlphaPerFrame;
        private float _alpha;
        private Vector2f _acceleration;
        private Vector2f _velocity;
        private int _timeToLive;
        private Vector2f _position;

        public float changeAlphaPerFrame { get { return _changeAlphaPerFrame; } set { _changeAlphaPerFrame = value; } }
        public float alpha { get { return _alpha; } set { _alpha = value; } }
        public Vector2f acceleration { get { return _acceleration; } set { _acceleration = value; } }
        public Vector2f velocity { get { return _velocity; } set { _velocity = value; } }
        public int timeToLive { get { return _timeToLive; } set { _timeToLive = value; } }
        public Vector2f position { get { return _position; } set { _position = value; } }

        public BloodParticle(Vector2f position, Vector2f acceleration, Vector2f velocity, int timeToLive, float alpha, float changeAlphaPerFrame)
        {
            _position = position;
            _alpha = alpha;
            _changeAlphaPerFrame = changeAlphaPerFrame;
            _acceleration = acceleration;
            _velocity = velocity;
            _timeToLive = timeToLive;
        }
    }
}
