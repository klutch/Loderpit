using System;
using System.Collections.Generic;
using FarseerPhysics.Dynamics;
using Microsoft.Xna.Framework;
using Loderpit.Components;
using Loderpit.Managers;

namespace Loderpit
{
    public class Helpers
    {
        private static float _pi = (float)Math.PI;

        // Get a random number between two floats
        public static float randomBetween(Random rng, float low, float high)
        {
            float rLow = Math.Min(low, high);
            float rHigh = Math.Max(low, high);
            float range = rHigh - rLow;
            return (float)rng.NextDouble() * range + rLow;
        }

        // Convert degrees to radians
        public static float degToRad(float degrees)
        {
            return degrees * (_pi / 180f);
        }

        // Convert radians to degrees
        public static float radToDeg(float radians)
        {
            return radians * (180f / _pi) - 90;
        }

        // Find entities of a certain faction within a certain range
        public static List<int> findEntitiesWithinRange(int entityId, float radius, Faction factionToMatch)
        {
            PositionComponent positionComponent = EntityManager.getPositionComponent(entityId);
            List<int> entitiesWithFaction = EntityManager.getEntitiesPossessing(ComponentType.Faction);
            List<int> results = new List<int>();

            foreach (int targetEntityId in entitiesWithFaction)
            {
                PositionComponent targetPositionComponent;
                FactionComponent targetFactionComponent;
                Vector2 relative;

                // Skip self
                if (targetEntityId == entityId)
                {
                    continue;
                }

                targetPositionComponent = EntityManager.getPositionComponent(targetEntityId);
                targetFactionComponent = EntityManager.getFactionComponent(targetEntityId);
                relative = targetPositionComponent.position - positionComponent.position;

                // Check faction
                if (targetFactionComponent.faction != factionToMatch)
                {
                    continue;
                }

                // Check range
                if (relative.Length() <= radius)
                {
                    results.Add(targetEntityId);
                }
            }

            return results;
        }
    }
}
