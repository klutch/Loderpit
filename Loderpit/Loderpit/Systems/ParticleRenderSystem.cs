using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using SFML.Graphics;
using SFML.Window;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class ParticleRenderSystem : ISystem
    {
        private const int MAX_PARTICLES = 4000;
        private Particle[] _particles;
        private List<int> _livingParticles;
        private List<int> _particlesToKill;
        private Random _rng;
        private Texture _fluidTexture;
        private Texture _shotTrailTexture;
        private Color _fireColor;

        public SystemType systemType { get { return SystemType.ParticleRender; } }

        public ParticleRenderSystem()
        {
            _rng = new Random();
            _fireColor = new Color(255, 128, 0, 255);

            // Initialize particles
            _livingParticles = new List<int>();
            _particlesToKill = new List<int>();
            _particles = new Particle[MAX_PARTICLES];
            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                _particles[i] = new Particle();
            }

            // Textures
            _fluidTexture = ResourceManager.getResource<Texture>("fluid_particle");
            _shotTrailTexture = ResourceManager.getResource<Texture>("shot_trail");
        }

        // Find first dead particle
        private int findFirstDeadParticle()
        {
            for (int i = 0; i < MAX_PARTICLES; i++)
            {
                if (!_livingParticles.Contains(i))
                {
                    return i;
                }
            }

            return -1;
        }

        // Create particle
        public void createParticle(Texture texture, Color color, float worldWidth, float worldHeight, Vector2 position, Vector2 offset, Vector2 velocity, Vector2 acceleration, int timeToLive, float rotation, float rotationVelocity)
        {
            int particleId = findFirstDeadParticle();
            Particle particle = _particles[particleId];

            particle.acceleration = new Vector2f(acceleration.X, acceleration.Y);
            particle.velocity = new Vector2f(velocity.X, velocity.Y);
            particle.position = new Vector2f(position.X, position.Y);
            particle.color = color;
            particle.texture = texture;
            particle.shape.TextureRect = new IntRect(0, 0, (int)texture.Size.X, (int)texture.Size.Y);
            particle.timeToLive = timeToLive;
            particle.rotation = rotation;
            particle.rotationVelocity = rotationVelocity;
            particle.shape.Size = new Vector2f(worldWidth, worldHeight);
            particle.shape.Origin = new Vector2f(particle.shape.Size.X * offset.X, particle.shape.Size.Y * offset.Y);

            _livingParticles.Add(particleId);
        }

        // Add blood particle effect
        public void addBloodParticleEffect(Color color, Vector2 position, Vector2 force, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Vector2 offset = new Vector2(Helpers.randomBetween(_rng, -1f, 1f), Helpers.randomBetween(_rng, -1f, 1f));
                Texture texture = _fluidTexture;
                float scale = Helpers.randomBetween(_rng, 0.25f, 0.9f);

                createParticle(
                    texture,
                    color,
                    ((float)texture.Size.X / CameraSystem.ORIGINAL_SCALE) * scale,
                    ((float)texture.Size.Y / CameraSystem.ORIGINAL_SCALE) * scale,
                    position + offset * 0.4f,
                    new Vector2(0.5f, 0.5f),
                    (force + offset * 1.5f) + new Vector2(0, 1f),
                    new Vector2(0, 9.8f),
                    240,
                    0f,
                    0f);
            }
        }

        // Add shot trail
        public void addShotTrail(Color color, Vector2 pointA, Vector2 pointB)
        {
            Vector2 relative = pointB - pointA;
            float angleInRads = (float)Math.Atan2(relative.Y, relative.X);
            float rotation = Helpers.radToDeg(angleInRads) + 180;

            createParticle(_shotTrailTexture, color, 0.05f, relative.Length(), pointB, Vector2.Zero, Vector2.Zero, Vector2.Zero, 30, rotation, 0);
        }

        // Add fire particle
        public void addFireParticle(Vector2 position, int amount)
        {
            for (int i = 0; i < amount; i++)
            {
                Texture texture = _fluidTexture;
                float size = Helpers.randomBetween(_rng, 0.25f, 0.5f);
                Vector2 jitter = new Vector2(Helpers.randomBetween(_rng, -0.25f, 0.25f), Helpers.randomBetween(_rng, -1.5f, 0f));

                createParticle(texture, _fireColor, size, size, position + jitter, new Vector2(0.5f, 0.5f), new Vector2(jitter.X * 5f, 0f), new Vector2(0, -1.5f), 180, 0f, 0f);
            }
        }

        // Update
        public void update()
        {
            float dt = SystemManager.physicsSystem.isSlowMotion ? PhysicsSystem.SLOW_DT : PhysicsSystem.NORMAL_DT;

            // Update particle aspects
            for (int i = 0; i < _livingParticles.Count; i++)
            {
                int particleId = _livingParticles[i];
                Particle particle = _particles[particleId];

                // Handle slow motion
                if ((!SystemManager.physicsSystem.isSlowMotion) || (SystemManager.physicsSystem.isSlowMotion && SystemManager.physicsSystem.isReadyForSlowMotionTick))
                {
                    // Handle time to live
                    if (particle.timeToLive == 0)
                    {
                        _particlesToKill.Add(particleId);
                    }
                    else
                    {
                        particle.timeToLive--;
                    }

                    // Handle physics
                    particle.velocity += particle.acceleration * dt;
                    particle.position += particle.velocity * dt;
                    particle.rotation += particle.rotationVelocity;
                }
            }

            // Remove dead particles
            foreach (int particleId in _particlesToKill)
            {
                _livingParticles.Remove(particleId);
            }
            _particlesToKill.Clear();
        }

        // Draw
        public void draw()
        {
            foreach (int particleId in _livingParticles)
            {
                Game.window.Draw(_particles[particleId].shape);
            }
        }
    }
}
