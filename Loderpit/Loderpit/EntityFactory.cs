using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Microsoft.Xna.Framework;
using FarseerPhysics.Common;
using FarseerPhysics.Dynamics;
using FarseerPhysics.Dynamics.Contacts;
using FarseerPhysics.Dynamics.Joints;
using FarseerPhysics.Factories;
using FarseerRubeLoader;
using Loderpit.Formations;
using Loderpit.Components;
using Loderpit.Managers;
using Loderpit.Systems;
using Loderpit.Skills;

namespace Loderpit
{
    public class EntityFactory
    {
        private static StatsComponent getCharacterStats(int entityId, CharacterClass characterClass)
        {
            StatsComponent statsComponent = new StatsComponent(entityId, 10, 10, 10, 10, 10, 60);

            switch (characterClass)
            {
                case CharacterClass.Archer:
                    statsComponent.attackDelay = 100;
                    break;

                case CharacterClass.Engineer:
                    break;

                case CharacterClass.Defender:
                    statsComponent.baseHp = 20;
                    statsComponent.attackDelay = 80;
                    break;

                case CharacterClass.Fighter:
                    statsComponent.baseHp = 15;
                    break;

                case CharacterClass.Healer:
                    break;

                case CharacterClass.Mage:
                    break;
            }

            statsComponent.currentHp = SystemManager.statSystem.getMaxHp(statsComponent);

            return statsComponent;
        }

        private static List<Skill> getStartingSkills(CharacterClass characterClass)
        {
            List<Skill> skills = new List<Skill>();

            switch (characterClass)
            {
                case CharacterClass.Fighter:
                    skills.Add(new MeleeAttackSkill(1));
                    skills.Add(new KickSkill(1));
                    break;

                case CharacterClass.Archer:
                    skills.Add(new RangedAttackSkill(1, 8f, "bow_icon"));
                    skills.Add(new ShieldOfThornsSkill(1));
                    skills.Add(new PowerShotSkill(1, 10f));
                    break;

                case CharacterClass.Mage:
                    skills.Add(new RangedAttackSkill(1, 8f, "wand_icon"));
                    break;

                case CharacterClass.Engineer:
                    skills.Add(new ThrowRopeSkill(1, 6f));
                    skills.Add(new BuildBridgeSkill(1, 2f));
                    break;
            }

            return skills;
        }

        public static int createPlayerGroup(List<CharacterClass> characterClasses)
        {
            List<int> groupEntities = new List<int>();
            int groupEntityId = createGroup(groupEntities, new List<Formation>(new[] {new DefaultFormation(groupEntities, 0, 0)}));

            foreach (CharacterClass characterClass in characterClasses)
            {
                groupEntities.Add(createTeammate(characterClass, Vector2.Zero));
            }

            return groupEntityId;
        }

        public static int createGroup(List<int> entities, List<Formation> formations)
        {
            int groupEntityId = EntityManager.createEntity();

            EntityManager.addComponent(groupEntityId, new GroupComponent(groupEntityId, entities, formations));

            return groupEntityId;
        }

        private static int createTeammate(CharacterClass characterClass, Vector2 position)
        {
            int entityId = EntityManager.createEntity();
            CharacterComponent characterComponent;
            Vector2 feetOffset = new Vector2(0, 0.25f);
            Body body;
            Body feet;
            RevoluteJoint feetJoint;
            Fixture interactionSensor;
            World world = SystemManager.physicsSystem.world;

            body = BodyFactory.CreateRectangle(world, 0.5f, 0.5f, 0.5f, position);
            body.BodyType = BodyType.Dynamic;
            body.UserData = entityId;
            body.FixedRotation = true;
            body.Friction = 0f;
            body.UserData = entityId;
            body.CollisionCategories = (ushort)CollisionCategory.Teammate;
            body.CollidesWith = (ushort)CollisionCategory.Bridge | (ushort)CollisionCategory.Ground;

            feet = BodyFactory.CreateCircle(world, 0.25f, 1f, position + feetOffset);
            feet.BodyType = BodyType.Dynamic;
            feet.UserData = entityId;
            feet.Friction = 10f;
            feet.CollisionCategories = (ushort)CollisionCategory.Teammate;
            feet.CollidesWith = (ushort)CollisionCategory.Bridge | (ushort)CollisionCategory.Ground;

            feetJoint = new RevoluteJoint(body, feet, feetOffset, Vector2.Zero, false);
            feetJoint.MotorEnabled = true;
            feetJoint.MaxMotorTorque = 100f;
            feetJoint.MotorSpeed = 0f;
            world.AddJoint(feetJoint);

            interactionSensor = FixtureFactory.AttachCircle(0.5f, 0.00001f, body);
            interactionSensor.IsSensor = true;
            interactionSensor.CollisionCategories = (ushort)CollisionCategory.CharacterInteractionSensor;
            interactionSensor.CollidesWith = (ushort)CollisionCategory.CharacterInteractionReceptor;

            characterComponent = new CharacterComponent(entityId, body, feet, feetJoint, interactionSensor, characterClass);
            characterComponent.feet.OnCollision += new OnCollisionEventHandler(characterFeetOnCollision);
            characterComponent.feet.OnSeparation += new OnSeparationEventHandler(characterFeetOnSeparation);
            characterComponent.interactionSensor.OnCollision += new OnCollisionEventHandler(characterInteractionOnCollision);
            characterComponent.interactionSensor.OnSeparation += new OnSeparationEventHandler(characterInteractionOnSeparation);

            EntityManager.addComponent(entityId, characterComponent);
            EntityManager.addComponent(entityId, getCharacterStats(entityId, characterClass));
            EntityManager.addComponent(entityId, new PositionComponent(entityId, body));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new SkillsComponent(entityId, getStartingSkills(characterClass)));
            EntityManager.addComponent(entityId, new SpellEffectsComponent(entityId));
            EntityManager.addComponent(entityId, new FactionComponent(entityId, Faction.Player, Faction.Enemy));
            EntityManager.addComponent(entityId, new RenderHealthComponent(entityId));
            EntityManager.addComponent(entityId, new PerformingSkillsComponent(entityId));

            return entityId;
        }

        private static bool characterFeetOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            int entityIdA = (int)fixtureA.Body.UserData;
            CharacterComponent characterBodyComponent = EntityManager.getCharacterComponent(entityIdA);

            if (fixtureB.Body.UserData != null)
            {
                int entityIdB = (int)fixtureB.Body.UserData;
                GroundBodyComponent groundBodyComponent;
                FixedArray2<Vector2> points;
                Vector2 normal;

                if ((groundBodyComponent = EntityManager.getGroundBodyComponent(entityIdB)) != null)
                {
                    contact.GetWorldManifold(out normal, out points);
                    characterBodyComponent.groundContactCount++;
                }
            }
            return true;
        }

        private static void characterFeetOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            int entityIdA = (int)fixtureA.Body.UserData;
            CharacterComponent characterBodyComponent = EntityManager.getCharacterComponent(entityIdA);

            if (fixtureB.Body.UserData != null)
            {
                int entityIdB = (int)fixtureB.Body.UserData;

                GroundBodyComponent groundBodyComponent = EntityManager.getGroundBodyComponent(entityIdB);

                if (groundBodyComponent != null)
                {
                    characterBodyComponent.groundContactCount--;
                }
            }
        }

        private static bool characterInteractionOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            int entityIdA = (int)fixtureA.Body.UserData;

            if (fixtureB.Body.UserData != null)
            {
                int entityIdB = (int)fixtureB.Body.UserData;
                RopeComponent ropeComponent;
                RopeGrabComponent ropeGrabComponent;
                SendActivateObstacleComponent sendActivateObstacleComponent;
                DestructibleObstacleComponent destructibleObstacleComponent;

                if (contact.IsTouching)
                {
                    if ((ropeComponent = EntityManager.getRopeComponent(entityIdB)) != null)
                    {
                        // A character is touching an entity with a rope component
                        if ((ropeGrabComponent = EntityManager.getRopeGrabComponent(entityIdA)) == null)
                        {
                            SystemManager.characterSystem.grabRope(entityIdA, ropeComponent, fixtureB.Body);
                        }
                    }
                    else if ((sendActivateObstacleComponent = EntityManager.getSendActivateObstacleComponent(entityIdB)) != null)
                    {
                        // A character has entered an area designated to activate an obstacle
                        SystemManager.obstacleSystem.activateObstacle(sendActivateObstacleComponent);
                    }
                    else if ((destructibleObstacleComponent = EntityManager.getDestructibleObstacleComponent(entityIdB)) != null)
                    {
                        // A character has touched a destructible object
                        GroupComponent groupComponent = SystemManager.groupSystem.getGroupComponentContaining(entityIdA);

                        if (groupComponent != null)
                        {
                            SplitFormation existingSplitFormation = (SplitFormation)groupComponent.getFormation(FormationType.Split);

                            if (existingSplitFormation == null)
                            {
                                int slot = groupComponent.entities.IndexOf(entityIdA);
                                bool hitOnLeft = fixtureB.Body.Position.X < fixtureA.Body.Position.X;
                                SplitFormation formation = new SplitFormation(groupComponent.entities, destructibleObstacleComponent.body.Position.X, hitOnLeft ? slot : slot + 1);

                                groupComponent.addFormation(formation);
                                destructibleObstacleComponent.formationsToRemove.Add(groupComponent.entityId, formation);
                            }
                        }
                    }
                }
            }

            return true;
        }

        private static void characterInteractionOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
        }

        public static int createEnemy(CharacterClass characterClass, Vector2 position)
        {
            int entityId = EntityManager.createEntity();
            CharacterComponent characterComponent;
            Vector2 feetOffset = new Vector2(0, 0.25f);
            Body body;
            Body feet;
            RevoluteJoint feetJoint;
            Fixture interactionSensor;
            World world = SystemManager.physicsSystem.world;

            body = BodyFactory.CreateRectangle(world, 0.5f, 0.5f, 0.5f, position);
            body.BodyType = BodyType.Dynamic;
            body.UserData = entityId;
            body.FixedRotation = true;
            body.Friction = 0f;
            body.UserData = entityId;
            body.CollisionCategories = (ushort)CollisionCategory.Teammate;
            body.CollidesWith = (ushort)CollisionCategory.Bridge | (ushort)CollisionCategory.Ground;

            feet = BodyFactory.CreateCircle(world, 0.25f, 1f, position + feetOffset);
            feet.BodyType = BodyType.Dynamic;
            feet.UserData = entityId;
            feet.Friction = 10f;
            feet.CollisionCategories = (ushort)CollisionCategory.Teammate;
            feet.CollidesWith = (ushort)CollisionCategory.Bridge | (ushort)CollisionCategory.Ground;

            feetJoint = new RevoluteJoint(body, feet, feetOffset, Vector2.Zero, false);
            feetJoint.MotorEnabled = true;
            feetJoint.MaxMotorTorque = 100f;
            feetJoint.MotorSpeed = 0f;
            world.AddJoint(feetJoint);

            interactionSensor = FixtureFactory.AttachCircle(0.5f, 0.00001f, body);
            interactionSensor.IsSensor = true;
            interactionSensor.CollisionCategories = (ushort)CollisionCategory.CharacterInteractionSensor;
            interactionSensor.CollidesWith = (ushort)CollisionCategory.CharacterInteractionReceptor;

            characterComponent = new CharacterComponent(entityId, body, feet, feetJoint, interactionSensor, characterClass);
            /*characterComponent.feet.OnCollision += new OnCollisionEventHandler(characterFeetOnCollision);
            characterComponent.feet.OnSeparation += new OnSeparationEventHandler(characterFeetOnSeparation);
            characterComponent.interactionSensor.OnCollision += new OnCollisionEventHandler(characterInteractionOnCollision);
            characterComponent.interactionSensor.OnSeparation += new OnSeparationEventHandler(characterInteractionOnSeparation);*/

            EntityManager.addComponent(entityId, characterComponent);
            EntityManager.addComponent(entityId, getCharacterStats(entityId, characterClass));
            EntityManager.addComponent(entityId, new PositionComponent(entityId, body));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new SkillsComponent(entityId, new List<Skill>( new[] { new MeleeAttackSkill(1) })));
            EntityManager.addComponent(entityId, new SpellEffectsComponent(entityId));
            EntityManager.addComponent(entityId, new FactionComponent(entityId, Faction.Enemy, Faction.Player));
            EntityManager.addComponent(entityId, new RenderHealthComponent(entityId));
            EntityManager.addComponent(entityId, new PerformingSkillsComponent(entityId));

            return entityId;
        }

        public static int createRope(Vector2 position)
        {
            int entityId = EntityManager.createEntity();
            float segmentLength = 1.2f;
            int maxSegments = 18;
            Vector2 endPoint = position + new Vector2(0, segmentLength * maxSegments);
            World world = SystemManager.physicsSystem.world;
            Fixture closestFixture = null;
            int segmentCount;
            Vector2 relative;
            Vector2 ropeNormal;
            float angle;
            List<Body> bodies = new List<Body>();
            Body anchorBody = BodyFactory.CreateCircle(world, 0.2f, 1f, position);
            Vector2 offset = new Vector2(0, segmentLength / 2f);

            anchorBody.FixtureList[0].IsSensor = true;

            world.RayCast((f, p, n, fr) =>
            {
                IgnoreRopeRaycastComponent ignoreRaycast = EntityManager.getIgnoreRopeRaycastComponent((int)f.Body.UserData);

                if (ignoreRaycast == null)
                {
                    closestFixture = f;
                    endPoint = p;
                }
                else
                {
                    return -1;
                }
                return fr;
            },
            position,
            endPoint);

            relative = endPoint - position;
            segmentCount = (int)Math.Floor(relative.Length() / segmentLength);
            ropeNormal = Vector2.Normalize(relative);
            angle = (float)Math.Atan2(relative.Y, relative.X);

            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 segmentPosition = offset + position + ropeNormal * segmentLength * i;
                Body body = BodyFactory.CreateRectangle(world, segmentLength + 0.2f, 0.3f, 0.5f, segmentPosition);

                body.BodyType = BodyType.Dynamic;
                body.Rotation = angle;
                body.Friction = 0.5f;
                body.CollisionCategories = (ushort)CollisionCategory.Rope | (ushort)CollisionCategory.CharacterInteractionReceptor;
                body.CollidesWith = (ushort)CollisionCategory.Bridge | (ushort)CollisionCategory.Ground | (ushort)CollisionCategory.CharacterInteractionSensor;
                body.UserData = entityId;
                bodies.Add(body);

                if (i > 0)
                {
                    JointFactory.CreateRevoluteJoint(world, bodies[i - 1], bodies[i], new Vector2(-segmentLength / 2f, 0));
                }
                else
                {
                    JointFactory.CreateRevoluteJoint(world, anchorBody, bodies[0], new Vector2(-segmentLength / 2f, 0));
                }
            }

            EntityManager.addComponent(entityId, new RopeComponent(entityId, bodies, anchorBody, ClimbDirection.Down));

            return entityId;
        }

        public static int createBridge(Vector2 positionA, Vector2 positionB)
        {
            int entityId = EntityManager.createEntity();
            float idealSegmentLength = 1.4f;
            float actualSegmentLength = idealSegmentLength;
            World world = SystemManager.physicsSystem.world;
            Fixture closestFixture = null;
            int segmentCount;
            Vector2 relative;
            Vector2 ropeNormal;
            float angle;
            List<Body> bodies = new List<Body>();
            Vector2 finalPositionA = positionA;
            Vector2 finalPositionB = positionB;
            Body bodyB = null;
            Body bodyA = null;
            Vector2 offset;

            world.RayCast((f, p, n, fr) =>
            {
                IgnoreBridgeRaycastComponent ignoreRaycast = EntityManager.getIgnoreBridgeRaycastComponent((int)f.Body.UserData);

                if (ignoreRaycast == null)
                {
                    closestFixture = f;
                    finalPositionB = p;
                    bodyB = f.Body;
                }
                else
                {
                    return -1;
                }
                return fr;
            },
            positionA,
            positionB);

            world.RayCast((f, p, n, fr) =>
            {
                IgnoreBridgeRaycastComponent ignoreRaycast = EntityManager.getIgnoreBridgeRaycastComponent((int)f.Body.UserData);

                if (ignoreRaycast == null)
                {
                    closestFixture = f;
                    finalPositionA = p;
                    bodyA = f.Body;
                }
                else
                {
                    return -1;
                }
                return fr;
            },
            finalPositionB,
            positionA);

            if (bodyA == null || bodyB == null)
            {
                return -1;
            }

            relative = finalPositionB - finalPositionA;
            segmentCount = (int)Math.Floor(relative.Length() / idealSegmentLength);
            ropeNormal = Vector2.Normalize(relative);
            angle = (float)Math.Atan2(relative.Y, relative.X);
            actualSegmentLength = relative.Length() / (float)segmentCount;
            offset = ropeNormal * idealSegmentLength * 0.5f;

            for (int i = 0; i < segmentCount; i++)
            {
                Vector2 segmentPosition = offset + finalPositionA + ropeNormal * actualSegmentLength * i;
                Body body = BodyFactory.CreateRectangle(world, actualSegmentLength + 0.3f, 0.3f, 1f, segmentPosition);

                body.BodyType = BodyType.Dynamic;
                body.Rotation = angle;
                body.Friction = 1f;
                body.CollisionCategories = (ushort)CollisionCategory.Bridge;
                body.CollidesWith = 
                    (ushort)CollisionCategory.Rope |
                    (ushort)CollisionCategory.Bridge |
                    (ushort)CollisionCategory.Teammate |
                    (ushort)CollisionCategory.Ground;
                body.UserData = entityId;
                bodies.Add(body);

                if (i > 0)
                {
                    Vector2 worldAnchor = (bodies[i].Position + bodies[i - 1].Position) / 2f;
                    JointFactory.CreateRevoluteJoint(world, bodies[i], bodies[i - 1], worldAnchor, worldAnchor, true);
                }

                if (i == 0)
                {
                    JointFactory.CreateRevoluteJoint(world, bodyA, bodies[i], new Vector2(-actualSegmentLength / 2f, 0));
                }
                else if (i == segmentCount - 1)
                {
                    JointFactory.CreateRevoluteJoint(world, bodyB, bodies[i], new Vector2(actualSegmentLength / 2f, 0));
                }
            }

            EntityManager.addComponent(entityId, new BridgeComponent(entityId, bodies));

            return entityId;
        }

        public static int afterLoadBody(string name, Body body, CustomProperties customProperties, XElement bodyData)
        {
            bool activatesObstacle = false;
            bool ignoresRopeRaycast = false;
            bool ignoresBridgeRaycast = false;
            bool isDestructibleObstacle = false;
            bool isGround = false;
            bool isCeiling = false;
            bool isLevelEnd = false;
            int entityId = EntityManager.createEntity();

            body.UserData = entityId;
            body.CollisionCategories = (ushort)CollisionCategory.Ground;

            customProperties.tryGetBool("isGround", out isGround);
            customProperties.tryGetBool("isDestructibleObstacle", out isDestructibleObstacle);
            customProperties.tryGetBool("activatesObstacle", out activatesObstacle);
            customProperties.tryGetBool("ignoresRopeRaycast", out ignoresRopeRaycast);
            customProperties.tryGetBool("ignoresBridgeRaycast", out ignoresBridgeRaycast);
            customProperties.tryGetBool("isCeiling", out isCeiling);
            customProperties.tryGetBool("isLevelEnd", out isLevelEnd);

            EntityManager.addComponent(entityId, new PositionComponent(entityId, body));

            // GroundBody component
            if (isGround)
            {
                EntityManager.addComponent(entityId, new GroundBodyComponent(entityId, body));
                body.CollisionCategories = (ushort)CollisionCategory.Ground;
            }

            // Ceiling component
            if (isCeiling)
            {
                EntityManager.addComponent(entityId, new CeilingComponent(entityId, body));
                body.CollisionCategories = (ushort)CollisionCategory.Ceiling;
            }

            // DestructibleObstacle (with Vitals) components
            if (isDestructibleObstacle)
            {
                EntityManager.addComponent(entityId, new DestructibleObstacleComponent(entityId, body));
                EntityManager.addComponent(entityId, new StatsComponent(entityId, 10, 10, 10, 10, 10, 100));
                EntityManager.addComponent(entityId, new FactionComponent(entityId, Faction.Neutral, Faction.None));
                body.CollisionCategories = (ushort)(CollisionCategory.Ground | CollisionCategory.CharacterInteractionReceptor);
            }

            // IgnoresRopeRaycast and IgnoresBridgeRaycast components
            if (ignoresRopeRaycast)
            {
                EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));
            }
            if (ignoresBridgeRaycast)
            {
                EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            }

            // Level end
            if (isLevelEnd)
            {
                body.CollisionCategories = (ushort)CollisionCategory.CharacterInteractionReceptor;
                body.CollidesWith = (ushort)CollisionCategory.CharacterInteractionSensor;
                body.OnCollision += new OnCollisionEventHandler(levelEndOnCollision);
                body.OnSeparation += new OnSeparationEventHandler(levelEndOnSeparation);
            }

            return entityId;
        }

        // Handle collisions with the level end sensor
        private static bool levelEndOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            GroupComponent playerGroup = SystemManager.teamSystem.playerGroup;
            int entityIdB;

            // Skip if body doesn't have any user data
            if (fixtureB.Body.UserData == null)
            {
                return true;
            }

            // Ensure the fixtures are actually touching
            if (!contact.IsTouching)
            {
                return true;
            }

            // Get entityId
            entityIdB = (int)fixtureB.Body.UserData;

            // Make sure the entity is in the player's group
            if (!playerGroup.entities.Contains(entityIdB))
            {
                return true;
            }

            // Player character is touching the level end sensor
            EntityManager.addComponent(entityIdB, new IsTouchingEndLevelComponent(entityIdB));

            return true;
        }

        // Handle separations from the level end sensor
        private static void levelEndOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            GroupComponent playerGroup = SystemManager.teamSystem.playerGroup;
            int entityIdB;

            // Skip if body doesn't have any user data
            if (fixtureB.Body.UserData == null)
            {
                return;
            }

            // Get entityId
            entityIdB = (int)fixtureB.Body.UserData;

            // Make sure the entity is in the player's group
            if (!playerGroup.entities.Contains(entityIdB))
            {
                return;
            }

            // Player character has stopped touching the level end sensor
            EntityManager.removeComponent(entityIdB, ComponentType.IsTouchingEndLevel);
        }
    }
}
