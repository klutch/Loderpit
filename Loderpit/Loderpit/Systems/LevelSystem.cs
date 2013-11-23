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
            // Destroy all physical bodies
            foreach (Body body in SystemManager.physicsSystem.world.BodyList)
            {
                SystemManager.physicsSystem.world.RemoveBody(body);
            }
            SystemManager.physicsSystem.world.Step(1f / 60f);

            // Destroy all entities
            EntityManager.destroyAllEntities();
        }

        private void placeTemporaryEnemies()
        {
            Random rng = new Random();

            for (int i = 1; i < _map.moduleEndPointsCount; i++)
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

        // Handle level ending
        private void handleLevelEnding()
        {
            List<int> playerEntities = SystemManager.teamSystem.playerGroup.entities;
            List<int> playersTouchingEndLevel = EntityManager.getEntitiesPossessing(ComponentType.IsTouchingEndLevel);

            if (playerEntities.Count == playersTouchingEndLevel.Count)
            {
                List<CharacterClass> characterClasses = new List<CharacterClass>();

                // TODO: This should be removed in favor of loading saved data in Game.startInterLevelState()
                foreach (int entityId in playerEntities)
                {
                    characterClasses.Add(EntityManager.getCharacterComponent(entityId).characterClass);
                }

                // TODO: Saving should happen on this line
                Game.endLevelState();
                Game.startInterLevelState(characterClasses);
            }
        }

        public void update()
        {
            // Handle level ending
            handleLevelEnding();
        }
    }
}
