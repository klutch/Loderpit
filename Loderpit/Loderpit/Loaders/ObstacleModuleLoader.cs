using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Xml.Linq;
using FarseerRubeLoader;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Loderpit.Managers;
using Loderpit.Components;

namespace Loderpit.Loaders
{
    public class ObstacleModuleLoader : RubeLoader
    {
        private Level _map;
        private Random _rng;
        private Body _bodyThatSendsActivateObstacle;
        private List<Body> _bodiesThatReceiveActivateObstacleFall;

        public ObstacleModuleLoader(Level map)
            : base()
        {
            _map = map;
            _rng = new Random();
            _bodiesThatReceiveActivateObstacleFall = new List<Body>();
        }

        protected override void beforeLoadBodies(World world, CustomProperties customWorldProperties)
        {
            int moduleEndPointsCount = _map.moduleEndPointsCount;
            Vector2 size = _map.moduleEndPoints[moduleEndPointsCount - 1] - _map.moduleEndPoints[moduleEndPointsCount - 2];
            Vector2 randomFactor = new Vector2(Helpers.randomBetween(_rng, 0f, 1f), Helpers.randomBetween(_rng, 0f, 1f));
            bool attachToCeiling = false;

            _offset = _map.moduleEndPoints[_map.moduleEndPoints.Count - 2] + size * randomFactor;

            if (customWorldProperties.tryGetBool("attachToCeiling", out attachToCeiling) && attachToCeiling)
            {
                Vector2 point = Vector2.Zero;

                // Raycast for a ceiling using the random position
                world.RayCast(
                    (f, p, n, fr) => 
                        {
                            if (f.Body.UserData == null)
                            {
                                return -1;
                            }

                            if (EntityManager.getCeilingComponent((int)f.Body.UserData) != null)
                            {
                                _offset = p;
                                return fr;
                            }
                            else
                            {
                                return -1;
                            }
                        },
                    _offset + new Vector2(0f, 1000f),
                    _offset + new Vector2(0f, -1000f));
            }

            base.beforeLoadBodies(world, customWorldProperties);
        }

        protected override void afterLoadBody(string name, Body body, CustomProperties customProperties, XElement bodyData)
        {
            bool activatesObstacle = false;
            bool fallOnActivate = false;
            bool ignoreCeilingCollision = false;
            bool isDestructibleObstacle = false;
            int entityId = EntityFactory.afterLoadBody(name, body, customProperties, bodyData);

            customProperties.tryGetBool("activatesObstacle", out activatesObstacle);
            customProperties.tryGetBool("fallOnActivate", out fallOnActivate);
            customProperties.tryGetBool("ignoreCeilingCollision", out ignoreCeilingCollision);
            customProperties.tryGetBool("isDestructibleObstacle", out isDestructibleObstacle);

            // ActivatesObstacle components
            if (activatesObstacle)
            {
                body.CollisionCategories = (ushort)CollisionCategory.CharacterInteractionReceptor;
                body.CollidesWith = (ushort)CollisionCategory.CharacterInteractionSensor;
                _bodyThatSendsActivateObstacle = body;
            }

            // Fall on activate
            if (fallOnActivate)
            {
                _bodiesThatReceiveActivateObstacleFall.Add(body);
            }

            // Ignore ceiling collision
            if (ignoreCeilingCollision)
            {
                body.CollidesWith = (ushort)(CollisionCategory.All ^ CollisionCategory.Ceiling);
            }

            base.afterLoadBody(name, body, customProperties, bodyData);
        }

        protected override void afterLoadBodies()
        {
            List<int> receivingEntities = new List<int>();
            int senderEntityId = (int)_bodyThatSendsActivateObstacle.UserData;

            foreach (Body body in _bodiesThatReceiveActivateObstacleFall)
            {
                int entityId = (int)body.UserData;

                EntityManager.addComponent(entityId, new ReceiveActivateObstacleFallComponent(entityId, body));
                receivingEntities.Add(entityId);
            }

            EntityManager.addComponent(senderEntityId, new SendActivateObstacleComponent(senderEntityId, receivingEntities));

            // Reset ... TODO -- Maybe make reset() a callback in RubeLoader?
            _bodiesThatReceiveActivateObstacleFall.Clear();
            _bodyThatSendsActivateObstacle = null;

            base.afterLoadBodies();
        }
    }
}
