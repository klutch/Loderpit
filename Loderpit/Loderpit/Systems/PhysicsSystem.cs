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
        private const float WALK_STRENGTH = 10f;
        private World _world;

        public World world { get { return _world; } }
        public SystemType systemType { get { return SystemType.Physics; } }

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

        public void moveCharacters(List<int> characters)
        {
            foreach (int entityId in characters)
            {
                CharacterComponent characterBodyComponent = EntityManager.getCharacterComponent(entityId);
                float motorSpeed = getMotorSpeed(characterBodyComponent.movementSpeed);

                characterBodyComponent.feetJoint.MotorSpeed = motorSpeed;
                characterBodyComponent.movementSpeed = 0;
            }
        }

        public void update()
        {
            List<int> characterEntities = EntityManager.getEntitiesPossessing(ComponentType.Character);

            moveCharacters(characterEntities);

            _world.Step(Game.DT);
        }
    }
}
