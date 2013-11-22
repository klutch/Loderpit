﻿using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Loderpit.Managers;

namespace Loderpit.Systems
{
    public class LevelSystem : ISystem
    {
        private Map _map;

        public SystemType systemType { get { return SystemType.Level; } }

        public LevelSystem()
        {
            _map = new Map(SystemManager.physicsSystem.world);
            _map.generate();
            placeTemporaryEnemies();
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

        public void update()
        {

        }
    }
}