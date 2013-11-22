using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Factories;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using Microsoft.Xna.Framework;

namespace Loderpit.Components
{
    public class CharacterComponent : IComponent
    {
        private int _entityId;
        private Body _body;
        private Body _feet;
        private Fixture _interactionSensor;
        private RevoluteJoint _feetJoint;
        private int _groundContactCount;
        private int _movementSpeed;
        private CharacterClass _characterClass;

        public int entityId { get { return _entityId; } }
        public ComponentType componentType { get { return ComponentType.Character; } }
        public bool isOnGround { get { return _groundContactCount > 0; } }
        public Body body { get { return _body; } }
        public Body feet { get { return _feet; } }
        public Fixture interactionSensor { get { return _interactionSensor; } }
        public int movementSpeed { get { return _movementSpeed; } set { _movementSpeed = value; } }
        public int groundContactCount { get { return _groundContactCount; } set { _groundContactCount = value; } }
        public RevoluteJoint feetJoint { get { return _feetJoint; } }
        public CharacterClass characterClass { get { return _characterClass; } }

        public CharacterComponent(int entityId, Body body, Body feet, RevoluteJoint feetJoint, Fixture interactionSensor, CharacterClass characterClass)
        {
            _entityId = entityId;
            _body = body;
            _feet = feet;
            _feetJoint = feetJoint;
            _interactionSensor = interactionSensor;
            _characterClass = characterClass;
        }
    }
}
