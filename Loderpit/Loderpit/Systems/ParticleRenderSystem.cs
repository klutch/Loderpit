using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Particles;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class ParticleRenderSystem : ISystem
    {
        private const int MAX_PARTICLES = 4000;
        private List<Particle> _particles;
        private VertexArray _particleVertexArray;
        private Random _rng;
        private List<Particle> _particlesToRemove;

        public SystemType systemType { get { return SystemType.ParticleRender; } }

        public ParticleRenderSystem()
        {
            _rng = new Random();
            _particles = new List<Particle>(MAX_PARTICLES);
            _particlesToRemove = new List<Particle>();
            _particleVertexArray = new VertexArray(PrimitiveType.Triangles, MAX_PARTICLES);
        }

        // Add blood particle effect
        public void addBloodParticleEffect(Vector2 position, Vector2 force, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                _particles.Add(
                    new BloodParticle(
                        new Vector2f(position.X, position.Y),
                        new Vector2f(0f, 9.8f),
                        new Vector2f(Helpers.randomBetween(_rng, -1f, 1f), Helpers.randomBetween(_rng, -1, 1f)) * 0.75f + new Vector2f(force.X, force.Y),
                        180,
                        1f,
                        -0.001f));
            }
        }

        // Update
        public void update()
        {
            float dt = SystemManager.physicsSystem.isSlowMotion ? PhysicsSystem.SLOW_DT : PhysicsSystem.NORMAL_DT;

            // Update particle aspects
            for (int i = 0; i < _particles.Count; i++)
            {
                Particle particle = _particles[i];

                // Handle time to live -- handle slow motion
                if ((!SystemManager.physicsSystem.isSlowMotion) ||
                    (SystemManager.physicsSystem.isSlowMotion && SystemManager.physicsSystem.isReadyForSlowMotionTick))
                {
                    if (particle is ITimeToLive)
                    {
                        ITimeToLive timeToLiveParticle = particle as ITimeToLive;

                        if (timeToLiveParticle.timeToLive == 0)
                        {
                            _particlesToRemove.Add(particle);
                        }
                        else
                        {
                            timeToLiveParticle.timeToLive--;
                        }
                    }

                    // Handle acceleration
                    if (particle is IAcceleration)
                    {
                        IVelocity velocityParticle = particle as IVelocity;
                        IAcceleration accelerationParticle = particle as IAcceleration;

                        velocityParticle.velocity += accelerationParticle.acceleration * dt;
                    }
                }

                // Handle physics
                if (particle is IVelocity)
                {
                    IPosition positionParticle = particle as IPosition;
                    IVelocity velocityParticle = particle as IVelocity;

                    positionParticle.position += velocityParticle.velocity * dt;
                }

                // Handle alpha fade
                if (particle is IFadeAlpha)
                {
                    IFadeAlpha fadeAlphaParticle = particle as IFadeAlpha;

                    fadeAlphaParticle.alpha += fadeAlphaParticle.changeAlphaPerFrame;
                }
            }

            // Remove dead particles
            foreach (Particle particle in _particlesToRemove)
            {
                _particles.Remove(particle);
            }
            _particlesToRemove.Clear();

            // Build vertex array
            for (int i = 0; i < _particles.Count; i++)
            {
                IPosition position = _particles[i] as IPosition;
                IFadeAlpha fadeAlpha = _particles[i] as IFadeAlpha;
                float alpha = fadeAlpha == null ? 1f : fadeAlpha.alpha;
                Color color = new Color(255, 30, 30, (byte)(alpha * 255f));

                _particleVertexArray.Append(new Vertex(position.position + new Vector2f(-0.1f, -0.1f), color));
                _particleVertexArray.Append(new Vertex(position.position + new Vector2f(0.1f, -0.1f), color));
                _particleVertexArray.Append(new Vertex(position.position, color));
            }
        }

        // Draw
        public void draw()
        {
            Game.window.Draw(_particleVertexArray);
            _particleVertexArray.Clear();
        }
    }
}
