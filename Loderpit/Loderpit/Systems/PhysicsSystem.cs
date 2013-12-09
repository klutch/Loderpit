using System;
using System.Collections.Generic;
using FarseerPhysics;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using Microsoft.Xna.Framework;
using Loderpit.Managers;
using Loderpit.Components;

namespace Loderpit.Systems
{
    public class PhysicsSystem : ISystem
    {
        public const float SLOW_DT = 1f / 120f;
        public const float NORMAL_DT = 1f / 60f;
        private const float WALK_STRENGTH = 10f;
        private const int MAX_DELAY = 1;

        private World _world;
        private int _delay;
        private bool _isSlowMotion;

        public World world { get { return _world; } }
        public SystemType systemType { get { return SystemType.Physics; } }
        public int slowMotionDelay { get { return _delay; } }
        public bool isSlowMotion { get { return _isSlowMotion; } }

        public PhysicsSystem()
        {
            _world = new World(new Vector2(0, 10));
        }

        private float getMotorSpeed(int speed)
        {
            int absSpeed = Math.Abs(speed);
            float value = 0f;

            switch (absSpeed)
            {
                case 1: value = 6f; break;
                case 2: value = 9f; break;
                case 3: value = 12f; break;
                case 4: value = 16f; break;
                default: value = 0f; break;
            }

            return value * (speed < 0 ? -1 : 1);
        }

        // Move characters
        public void moveCharacters(List<int> characters)
        {
            foreach (int entityId in characters)
            {
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
                int characterMovementSpeed;
                ExternalMovementSpeedsComponent externalSpeeds = EntityManager.getExternalMovementSpeedsComponent(entityId);

                // Handle external movement speeds
                if (!externalSpeeds.tryGetExternalMovementSpeed(ExternalMovementSpeedType.ShieldBlock, out characterMovementSpeed))
                {
                    characterMovementSpeed = characterComponent.movementSpeed;
                }

                characterComponent.feetJoint.MotorSpeed = getMotorSpeed(characterMovementSpeed);
            }
        }

        // Correct characters feet
        private void correctCharactersFeet(List<int> entities)
        {
            foreach (int entityId in entities)
            {
                CharacterComponent characterComponent = EntityManager.getCharacterComponent(entityId);
                Vector2 relative = characterComponent.body.Position - characterComponent.feet.Position;

                if (relative.Length() >= 0.4f)
                {
                    characterComponent.feet.Position = characterComponent.body.Position;
                }
            }
        }
        
        // Handle slow motion delay
        private void handleSlowMotionDelay()
        {
            if (_isSlowMotion)
            {
                if (_delay == 0)
                {
                    _delay = MAX_DELAY;
                }
                else
                {
                    _delay--;
                }
            }
        }

        // Update
        public void update()
        {
            List<int> characterEntities = EntityManager.getEntitiesPossessing(ComponentType.Character);

            // Determine slow-motion status
            _isSlowMotion = EntityManager.getEntitiesPossessing(ComponentType.SlowMotion).Count > 0;

            // Handle slow motion delay
            handleSlowMotionDelay();

            // Move characters
            moveCharacters(characterEntities);

            // Correct characters feet
            correctCharactersFeet(characterEntities);

            // Step physics simulation
            _world.Step(_isSlowMotion ? SLOW_DT : NORMAL_DT);
        }
    }
}
