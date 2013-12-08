﻿using System;
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
using Loderpit.Components.SpellEffects;
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

            return statsComponent;
        }

        private static List<Skill> getStartingSkills(int entityId, CharacterClass characterClass)
        {
            List<Skill> skills = new List<Skill>();

            switch (characterClass)
            {
                case CharacterClass.Fighter:
                    skills.Add(new MeleeAttackSkill(entityId, 1));
                    skills.Add(new KickSkill(entityId, 1));
                    skills.Add(new PowerSwingSkill(entityId, 1));
                    skills.Add(new BloodletterSkill(entityId, 1));
                    skills.Add(new FatalitySkill(entityId, 1));
                    break;

                case CharacterClass.Defender:
                    skills.Add(new BlockSkill(entityId, 1));
                    skills.Add(new ShieldBashSkill(entityId, 1, new Vector2(1, -0.5f)));    // TODO: find a better way of determining the normal
                    skills.Add(new SpikedShieldSkill(entityId, 1));
                    break;

                case CharacterClass.Archer:
                    skills.Add(new RangedAttackSkill(entityId, 1, "bow_icon"));
                    skills.Add(new ShieldOfThornsSkill(entityId, 1));
                    skills.Add(new PowerShotSkill(entityId, 1));
                    skills.Add(new DeadeyeSkill(entityId, 1));
                    break;

                case CharacterClass.Mage:
                    skills.Add(new RangedAttackSkill(entityId, 1, "wand_icon"));
                    skills.Add(new IgniteSkill(entityId, 1));
                    skills.Add(new FireballSkill(entityId, 1));
                    skills.Add(new FlameAuraSkill(entityId, 1));
                    skills.Add(new RainOfFireSkill(entityId, 1));
                    break;

                case CharacterClass.Engineer:
                    skills.Add(new ThrowRopeSkill(entityId, 1));
                    skills.Add(new BuildBridgeSkill(entityId, 1));
                    skills.Add(new ProximityMineSkill(entityId, 1));
                    break;

                case CharacterClass.Healer:
                    skills.Add(new HealSkill(entityId, 1));
                    skills.Add(new HealingBlastSkill(entityId, 1));
                    skills.Add(new InfusionSkill(entityId, 1));
                    skills.Add(new DispelSkill(entityId, 1));
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
            World world = SystemManager.physicsSystem.world;

            body = BodyFactory.CreateRectangle(world, 0.5f, 0.5f, 0.5f, position);
            body.BodyType = BodyType.Dynamic;
            body.UserData = entityId;
            body.FixedRotation = true;
            body.Friction = 0f;
            body.UserData = entityId;
            body.CollisionCategories = (ushort)CollisionCategory.Characters;

            feet = BodyFactory.CreateCircle(world, 0.25f, 1f, position + feetOffset);
            feet.BodyType = BodyType.Dynamic;
            feet.UserData = entityId;
            feet.Friction = 5f;
            feet.CollisionCategories = (ushort)CollisionCategory.CharacterFeet;
            feet.CollidesWith = (ushort)CollisionCategory.Terrain;

            feetJoint = new RevoluteJoint(body, feet, feetOffset, Vector2.Zero, false);
            feetJoint.MotorEnabled = true;
            feetJoint.MaxMotorTorque = 100f;
            feetJoint.MotorSpeed = 0f;
            world.AddJoint(feetJoint);

            characterComponent = new CharacterComponent(entityId, body, feet, feetJoint, characterClass);
            characterComponent.body.OnCollision += new OnCollisionEventHandler(playerCharacterBodyOnCollision);
            characterComponent.feet.OnCollision += new OnCollisionEventHandler(playerCharacterFeetOnCollision);
            characterComponent.feet.OnSeparation += new OnSeparationEventHandler(playerCharacterFeetOnSeparation);

            EntityManager.addComponent(entityId, characterComponent);
            EntityManager.addComponent(entityId, getCharacterStats(entityId, characterClass));
            EntityManager.addComponent(entityId, new PositionComponent(entityId, body));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new SkillsComponent(entityId, getStartingSkills(entityId, characterClass)));
            EntityManager.addComponent(entityId, new FactionComponent(entityId, Faction.Player, Faction.Enemy));
            EntityManager.addComponent(entityId, new RenderHealthComponent(entityId));
            EntityManager.addComponent(entityId, new PerformingSkillsComponent(entityId));
            EntityManager.addComponent(entityId, new ExternalMovementSpeedsComponent(entityId));
            EntityManager.addComponent(entityId, new AffectedBySpellEntitiesComponent(entityId));

            return entityId;
        }

        private static bool playerCharacterBodyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            int entityIdA = (int)fixtureA.Body.UserData;
            int entityIdB;
            CharacterComponent characterComponentB;
            RopeComponent ropeComponentB;
            RopeGrabComponent ropeGrabComponentA;
            SendActivateObstacleComponent sendActivateObstacleComponentB;
            DestructibleObstacleComponent destructibleObstacleComponentB;

            // Skip fixtures without userdata
            if (fixtureB.Body.UserData == null)
            {
                return true;
            }

            entityIdB = (int)fixtureB.Body.UserData;

            // Don't collide with other characters
            if ((characterComponentB = EntityManager.getCharacterComponent(entityIdB)) != null)
            {
                return false;
            }

            if ((ropeComponentB = EntityManager.getRopeComponent(entityIdB)) != null)
            {
                // A character is touching an entity with a rope component
                if ((ropeGrabComponentA = EntityManager.getRopeGrabComponent(entityIdA)) == null)
                {
                    SystemManager.characterSystem.grabRope(entityIdA, ropeComponentB, fixtureB.Body);
                    return false;
                }
            }
            else if ((sendActivateObstacleComponentB = EntityManager.getSendActivateObstacleComponent(entityIdB)) != null)
            {
                // A character has entered an area designated to activate an obstacle
                SystemManager.obstacleSystem.activateObstacle(sendActivateObstacleComponentB);
            }
            else if ((destructibleObstacleComponentB = EntityManager.getDestructibleObstacleComponent(entityIdB)) != null)
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
                        SplitFormation formation = new SplitFormation(groupComponent.entities, destructibleObstacleComponentB.body.Position.X, hitOnLeft ? slot : slot + 1);

                        groupComponent.addFormation(formation);
                        destructibleObstacleComponentB.formationsToRemove.Add(groupComponent.entityId, formation);
                    }
                }
            }

            return true;
        }

        private static bool playerCharacterFeetOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            int entityIdA = (int)fixtureA.Body.UserData;
            CharacterComponent characterComponentA = EntityManager.getCharacterComponent(entityIdA);

            if (fixtureB.Body.UserData != null)
            {
                int entityIdB = (int)fixtureB.Body.UserData;
                GroundBodyComponent groundBodyComponent = EntityManager.getGroundBodyComponent(entityIdB);
                CharacterComponent characterComponentB = EntityManager.getCharacterComponent(entityIdB);
                FixedArray2<Vector2> points;
                Vector2 normal;

                // Ground contacts
                if (groundBodyComponent != null)
                {
                    contact.GetWorldManifold(out normal, out points);
                    characterComponentA.groundContactCount++;
                    return true;
                }

                // Don't collide with other characters
                if (characterComponentB != null)
                {
                    return false;
                }
            }
            return true;
        }

        private static void playerCharacterFeetOnSeparation(Fixture fixtureA, Fixture fixtureB)
        {
            int entityIdA = (int)fixtureA.Body.UserData;
            CharacterComponent characterBodyComponent = EntityManager.getCharacterComponent(entityIdA);

            if (fixtureB.Body.UserData != null)
            {
                int entityIdB = (int)fixtureB.Body.UserData;
                GroundBodyComponent groundBodyComponent = EntityManager.getGroundBodyComponent(entityIdB);

                // Skip if no ground body component
                if (groundBodyComponent == null)
                {
                    return;
                }

                characterBodyComponent.groundContactCount--;
            }
        }

        public static int createEnemy(CharacterClass characterClass, Vector2 position)
        {
            int entityId = EntityManager.createEntity();
            Vector2 feetOffset = new Vector2(0, 0.25f);
            Body body;
            Body feet;
            RevoluteJoint feetJoint;
            World world = SystemManager.physicsSystem.world;

            body = BodyFactory.CreateRectangle(world, 0.5f, 0.5f, 0.5f, position);
            body.BodyType = BodyType.Dynamic;
            body.UserData = entityId;
            body.FixedRotation = true;
            body.Friction = 0f;
            body.UserData = entityId;
            body.CollisionCategories = (ushort)CollisionCategory.Characters;

            feet = BodyFactory.CreateCircle(world, 0.25f, 1f, position + feetOffset);
            feet.BodyType = BodyType.Dynamic;
            feet.UserData = entityId;
            feet.Friction = 5f;
            feet.CollisionCategories = (ushort)CollisionCategory.CharacterFeet;
            feet.CollidesWith = (ushort)CollisionCategory.Terrain;

            feetJoint = new RevoluteJoint(body, feet, feetOffset, Vector2.Zero, false);
            feetJoint.MotorEnabled = true;
            feetJoint.MaxMotorTorque = 100f;
            feetJoint.MotorSpeed = 0f;
            world.AddJoint(feetJoint);

            body.OnCollision += new OnCollisionEventHandler(enemyCharacterBodyOnCollision);
            feet.OnCollision += new OnCollisionEventHandler(enemyCharacterFeetOnCollision);

            EntityManager.addComponent(entityId, new CharacterComponent(entityId, body, feet, feetJoint, characterClass));
            EntityManager.addComponent(entityId, new StatsComponent(entityId, 6, 6, 10, 1, 1, 120, "d3"));
            EntityManager.addComponent(entityId, new PositionComponent(entityId, body));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new SkillsComponent(entityId, new List<Skill>( new[] { new MeleeAttackSkill(entityId, 1) })));
            EntityManager.addComponent(entityId, new FactionComponent(entityId, Faction.Enemy, Faction.Player));
            EntityManager.addComponent(entityId, new RenderHealthComponent(entityId));
            EntityManager.addComponent(entityId, new PerformingSkillsComponent(entityId));
            EntityManager.addComponent(entityId, new ExternalMovementSpeedsComponent(entityId));
            EntityManager.addComponent(entityId, new AffectedBySpellEntitiesComponent(entityId));

            return entityId;
        }

        static bool enemyCharacterFeetOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            int entityIdA = (int)fixtureA.Body.UserData;
            int entityIdB;
            CharacterComponent characterComponentB;

            // Skip fixtures without userdata
            if (fixtureB.Body.UserData == null)
            {
                return true;
            }

            entityIdB = (int)fixtureB.Body.UserData;

            // Don't collide with other characters
            if ((characterComponentB = EntityManager.getCharacterComponent(entityIdB)) != null)
            {
                return false;
            }

            return true;
        }

        private static bool enemyCharacterBodyOnCollision(Fixture fixtureA, Fixture fixtureB, Contact contact)
        {
            int entityIdA = (int)fixtureA.Body.UserData;
            int entityIdB;
            CharacterComponent characterComponentB;

            // Skip fixtures without userdata
            if (fixtureB.Body.UserData == null)
            {
                return true;
            }

            entityIdB = (int)fixtureB.Body.UserData;

            // Don't collide with other characters
            if ((characterComponentB = EntityManager.getCharacterComponent(entityIdB)) != null)
            {
                // Make an exception for fixtures that are a shield
                if (fixtureB.UserData != null && (SpecialFixtureType)fixtureB.UserData == SpecialFixtureType.Shield)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
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
                body.UserData = entityId;
                body.CollisionCategories = (ushort)CollisionCategory.Terrain;
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
                body.CollisionCategories = (ushort)CollisionCategory.Terrain;
                EntityManager.addComponent(entityId, new GroundBodyComponent(entityId, body));
            }

            // Ceiling component
            if (isCeiling)
            {
                body.CollisionCategories = (ushort)CollisionCategory.Terrain;
                EntityManager.addComponent(entityId, new CeilingComponent(entityId, body));
            }

            // DestructibleObstacle (with Vitals) components
            if (isDestructibleObstacle)
            {
                EntityManager.addComponent(entityId, new DestructibleObstacleComponent(entityId, body));
                EntityManager.addComponent(entityId, new StatsComponent(entityId, 10, 10, 0, 0, 0, 100));
                EntityManager.addComponent(entityId, new FactionComponent(entityId, Faction.Neutral, Faction.None));
                EntityManager.addComponent(entityId, new AffectedBySpellEntitiesComponent(entityId));
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
                body.CollidesWith = (ushort)CollisionCategory.Characters;
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

        // Create proximity mine
        public static int createProximityMine(Vector2 position, Faction hostileFaction, float radius, float force, string damageDie)
        {
            int entityId = EntityManager.createEntity();
            Body body = BodyFactory.CreateCircle(SystemManager.physicsSystem.world, 3f, 1f, position);
            TimedExplosionComponent timedExplosionComponent = new TimedExplosionComponent(entityId, 60, radius, force, damageDie);
            PhysicsComponent physicsComponent = new PhysicsComponent(entityId);

            body.UserData = entityId;
            body.BodyType = BodyType.Static;
            body.IsSensor = true;
            body.CollidesWith = (ushort)CollisionCategory.Characters;
            body.OnCollision += new OnCollisionEventHandler((fixtureA, fixtureB, contact) =>
            {
                int entityIdB;
                FactionComponent factionComponentB;

                // Skip fixtures without a user data
                if (fixtureB.Body.UserData == null)
                {
                    return false;
                }

                entityIdB = (int)fixtureB.Body.UserData;

                // Skip if no faction component
                if ((factionComponentB = EntityManager.getFactionComponent(entityIdB)) == null)
                {
                    return false;
                }

                // Check faction
                if (hostileFaction != factionComponentB.faction)
                {
                    return false;
                }

                // Activate timed explosion component
                timedExplosionComponent.active = true;
                return false;
            });

            physicsComponent.bodies.Add(body);

            EntityManager.addComponent(entityId, timedExplosionComponent);
            EntityManager.addComponent(entityId, physicsComponent);
            EntityManager.addComponent(entityId, new PositionComponent(entityId, body));
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));

            return entityId;
        }

        // Deadeye spell entity
        public static int createDeadeyeSpell(int targetEntityId, int attackDieMod, float radius, List<Faction> factionsToAffect)
        {
            int entityId = EntityManager.createEntity();
            TrackEntityPositionComponent trackEntityPositionComponent = new TrackEntityPositionComponent(entityId, targetEntityId);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId, factionsToAffect);
            StatModifierComponent statModifierComponent = new StatModifierComponent(entityId);
            AreaOfEffectComponent areaOfEffectComponent;
            Body sensor = BodyFactory.CreateCircle(SystemManager.physicsSystem.world, radius, 1f);

            sensor.UserData = entityId;
            sensor.CollidesWith = (ushort)CollisionCategory.None;
            sensor.BodyType = BodyType.Static;
            areaOfEffectComponent = new AreaOfEffectComponent(entityId, sensor);

            statModifierComponent.attackDieMod = attackDieMod;

            EntityManager.addComponent(entityId, trackEntityPositionComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);
            EntityManager.addComponent(entityId, statModifierComponent);
            EntityManager.addComponent(entityId, areaOfEffectComponent);
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));

            return entityId;
        }

        // Generic dot spell -- TODO: Incorporate the type of damage
        public static int createDoTSpell(int targetEntityId, string damageDie, int tickDelay, int tickCount)
        {
            int entityId = EntityManager.createEntity();
            FactionComponent factionComponent = EntityManager.getFactionComponent(targetEntityId);
            DamageOverTimeComponent damageOverTimeComponent = new DamageOverTimeComponent(entityId, damageDie, tickDelay);
            TimeToLiveComponent timeToLiveComponent = new TimeToLiveComponent(entityId, tickCount * tickDelay);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(targetEntityId);
            DispellableComponent dispellableComponent = new DispellableComponent(entityId, new List<Faction>(new [] { factionComponent.faction }));

            affectedEntitiesComponent.entities.Add(targetEntityId);
            affectedBySpellEntitiesComponent.spellEntities.Add(entityId);
            
            EntityManager.addComponent(entityId, damageOverTimeComponent);
            EntityManager.addComponent(entityId, timeToLiveComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);
            EntityManager.addComponent(entityId, dispellableComponent);

            return entityId;
        }

        // Shield of thorns spell entity
        public static int createShieldOfThornsSpell(int targetEntityId, string damageDie, float radius, List<Faction> factionsToAffect)
        {
            int entityId = EntityManager.createEntity();
            TrackEntityPositionComponent trackEntityPositionComponent = new TrackEntityPositionComponent(entityId, targetEntityId);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId, factionsToAffect);
            DamageShieldComponent damageShieldComponent = new DamageShieldComponent(entityId, damageDie);
            AreaOfEffectComponent areaOfEffectComponent;
            Body sensor = BodyFactory.CreateCircle(SystemManager.physicsSystem.world, radius, 1f);

            sensor.UserData = entityId;
            sensor.CollidesWith = (ushort)CollisionCategory.None;
            sensor.BodyType = BodyType.Static;
            areaOfEffectComponent = new AreaOfEffectComponent(entityId, sensor);

            EntityManager.addComponent(entityId, trackEntityPositionComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);
            EntityManager.addComponent(entityId, damageShieldComponent);
            EntityManager.addComponent(entityId, areaOfEffectComponent);
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));

            return entityId;
        }

        // Spiked shield spell entity
        public static int createSpikedShieldSpell(int targetEntityId, string damageDie, float radius, List<Faction> factionsToAffect)
        {
            int entityId = EntityManager.createEntity();
            TrackEntityPositionComponent trackEntityPositionComponent = new TrackEntityPositionComponent(entityId, targetEntityId);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId, factionsToAffect);
            DamageOverTimeComponent damageOverTimeComponent = new DamageOverTimeComponent(entityId, damageDie, 60);
            AreaOfEffectComponent areaOfEffectComponent;
            Body sensor = BodyFactory.CreateCircle(SystemManager.physicsSystem.world, radius, 1f);

            sensor.UserData = entityId;
            sensor.CollidesWith = (ushort)CollisionCategory.None;
            sensor.BodyType = BodyType.Static;
            areaOfEffectComponent = new AreaOfEffectComponent(entityId, sensor);

            EntityManager.addComponent(entityId, trackEntityPositionComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);
            EntityManager.addComponent(entityId, damageOverTimeComponent);
            EntityManager.addComponent(entityId, areaOfEffectComponent);
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));

            return entityId;
        }

        // Create general proc spell
        public static int createProcSpell(int targetEntityId, Action<Skill, int, int> onHitOther = null, Action<Skill, int, int> onHitByOther = null)
        {
            int entityId = EntityManager.createEntity();
            ProcComponent procComponent = new ProcComponent(entityId, onHitOther, onHitByOther);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(targetEntityId);

            affectedEntitiesComponent.entities.Add(targetEntityId);
            affectedBySpellEntitiesComponent.spellEntities.Add(entityId);

            EntityManager.addComponent(entityId, procComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);

            return entityId;
        }

        // Create infusion spell
        public static int createInfusionSpell(int targetEntityId, int maxHpMod, int strengthMod, int armorClassMod, int timeToLive)
        {
            int entityId = EntityManager.createEntity();
            TimeToLiveComponent timeToLiveComponent = new TimeToLiveComponent(entityId, timeToLive);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId);
            AffectedBySpellEntitiesComponent affectedBySpellEntitiesComponent = EntityManager.getAffectedBySpellEntitiesComponent(targetEntityId);
            StatModifierComponent statModifierComponent = new StatModifierComponent(entityId);

            affectedEntitiesComponent.entities.Add(targetEntityId);
            affectedBySpellEntitiesComponent.spellEntities.Add(entityId);

            statModifierComponent.maxHpMod = maxHpMod;
            statModifierComponent.strengthMod = strengthMod;
            statModifierComponent.armorClassMod = armorClassMod;

            EntityManager.addComponent(entityId, timeToLiveComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);
            EntityManager.addComponent(entityId, statModifierComponent);

            return entityId;
        }

        // Create flame aura spell
        public static int createFlameAuraSpell(int targetId, float radius, string chanceToProc, string damageDie, int tickDelay, int tickCount, List<Faction> factionsToAffect)
        {
            int entityId = EntityManager.createEntity();
            PositionComponent positionComponent = EntityManager.getPositionComponent(targetId);
            TrackEntityPositionComponent trackEntityPositionComponent = new TrackEntityPositionComponent(entityId, targetId);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId, factionsToAffect);
            AreaOfEffectComponent areaOfEffectComponent;
            Body sensor = BodyFactory.CreateCircle(SystemManager.physicsSystem.world, radius, 1f, positionComponent.position);
            Action<Skill, int, int> onHitOther;

            sensor.UserData = entityId;
            sensor.BodyType = BodyType.Static;
            sensor.CollidesWith = (ushort)CollisionCategory.None;
            areaOfEffectComponent = new AreaOfEffectComponent(entityId, sensor);

            onHitOther = (skill, attackerId, defenderId) =>
                {
                    if (Roller.roll(chanceToProc) == 1)
                    {
                        createDoTSpell(defenderId, damageDie, tickDelay, tickCount);
                    }
                };

            EntityManager.addComponent(entityId, trackEntityPositionComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);
            EntityManager.addComponent(entityId, areaOfEffectComponent);
            EntityManager.addComponent(entityId, new ProcComponent(entityId, onHitOther, null));
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));

            return entityId;
        }

        // Create rain of fire spell
        public static int createRainOfFireSpell(Vector2 position, float width, string damageDie, int tickDelay, int tickCount, List<Faction> factionsToAffect)
        {
            int entityId = EntityManager.createEntity();
            DamageOverTimeComponent damageOverTimeComponent = new DamageOverTimeComponent(entityId, damageDie, tickDelay);
            TimeToLiveComponent timeToLiveComponent = new TimeToLiveComponent(entityId, tickDelay * tickCount);
            AffectedEntitiesComponent affectedEntitiesComponent = new AffectedEntitiesComponent(entityId, factionsToAffect);
            Body sensor = BodyFactory.CreateRectangle(SystemManager.physicsSystem.world, width, 100f, 1f, position);
            AreaOfEffectComponent areaOfEffectComponent = new AreaOfEffectComponent(entityId, sensor);

            sensor.UserData = entityId;
            sensor.BodyType = BodyType.Static;
            sensor.CollidesWith = (ushort)CollisionCategory.None;

            EntityManager.addComponent(entityId, damageOverTimeComponent);
            EntityManager.addComponent(entityId, timeToLiveComponent);
            EntityManager.addComponent(entityId, affectedEntitiesComponent);
            EntityManager.addComponent(entityId, areaOfEffectComponent);
            EntityManager.addComponent(entityId, new IgnoreBridgeRaycastComponent(entityId));
            EntityManager.addComponent(entityId, new IgnoreRopeRaycastComponent(entityId));

            return entityId;
        }
    }
}
