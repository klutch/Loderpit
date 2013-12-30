using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using FarseerPhysics.Dynamics;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class LevelSystem : ISystem
    {
        private Level _map;

        public SystemType systemType { get { return SystemType.Level; } }

        public LevelSystem()
        {
        }

        public void generateLevel(int iterations = 16)
        {
            _map = new Level(SystemManager.physicsSystem.world);
            _map.generate(iterations);
            placeTemporaryEnemies();
        }

        // Called from Game.endLevelState() -- Unloads the level and destroys all entities
        public void unload()
        {
            // Destroy all entities
            EntityManager.destroyAllEntities();
            SystemManager.physicsSystem.world.Step(1f / 60f);

            // Destroy left over bodies
            foreach (Body body in SystemManager.physicsSystem.world.BodyList)
            {
                SystemManager.physicsSystem.world.RemoveBody(body);
            }
            SystemManager.physicsSystem.world.Step(1f / 60f);
        }

        private void placeTemporaryEnemies()
        {
            Random rng = new Random();
            int numEntitiesPerSegment = 4;

            for (int i = 1; i < _map.moduleEndPointsCount - 1; i++)
            {
                for (int j = 0; j < numEntitiesPerSegment; j++)
                {
                    float x = Helpers.randomBetween(rng, _map.moduleEndPoints[i - 1].X, _map.moduleEndPoints[i].X);
                    float y = 0;
                    bool hit = false;

                    SystemManager.physicsSystem.world.RayCast((f, p, n, fr) =>
                    {
                        if (f.Body.UserData == null)
                        {
                            return -1;
                        }

                        if (EntityManager.getGroundBodyComponent((int)f.Body.UserData) != null)
                        {
                            y = p.Y;
                            hit = true;
                            return fr;
                        }
                        else
                        {
                            return -1;
                        }
                    },
                    new Vector2(x, -1000f),
                    new Vector2(x, 1000f));

                    if (hit)
                    {
                        EntityFactory.createEnemy(CharacterClass.Fighter, new Vector2(x, y - 1f));
                    }
                }
            }
        }

        // Handle level ending
        private void handleLevelEnding()
        {
            List<int> playerEntities = SystemManager.teamSystem.playerGroup.entities;
            List<int> playersTouchingEndLevel = EntityManager.getEntitiesPossessing(ComponentType.IsTouchingEndLevel);

            if (playerEntities.Count == playersTouchingEndLevel.Count)
            {
                Game.endLevelState();
                Game.startInterLevelState(PlayerDataManager.lastLoadedLevelUid);
            }
        }

        public void update()
        {
            // Handle level ending
            handleLevelEnding();
        }
    }
}
