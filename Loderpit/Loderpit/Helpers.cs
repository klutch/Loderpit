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
        public static List<int> findEntitiesWithinRange(Vector2 position, float radius, Faction factionToMatch, int entityToSkip = -1)
        {
            return findEntitiesWithinRange(position, radius, new List<Faction>(new[] { factionToMatch }), entityToSkip);
        }

        public static List<int> findEntitiesWithinRange(Vector2 position, float radius, List<Faction> factionsToMatch, int entityToSkip = -1)
        {
            List<int> entitiesWithFaction = EntityManager.getEntitiesPossessing(ComponentType.Faction);
            List<int> results = new List<int>();

            foreach (int targetEntityId in entitiesWithFaction)
            {
                PositionComponent targetPositionComponent;
                FactionComponent targetFactionComponent;
                Vector2 relative;
                bool foundMatchingFaction = false;

                // Skip entity
                if (entityToSkip != -1 && entityToSkip == targetEntityId)
                {
                    continue;
                }

                targetPositionComponent = EntityManager.getPositionComponent(targetEntityId);
                targetFactionComponent = EntityManager.getFactionComponent(targetEntityId);
                relative = targetPositionComponent.position - position;

                // Check factions
                foreach (Faction factionToMatch in factionsToMatch)
                {
                    if (targetFactionComponent.faction == factionToMatch)
                    {
                        foundMatchingFaction = true;
                        break;
                    }
                }

                // Skip target entity id if no matching factions found
                if (!foundMatchingFaction)
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

        // Find entity of a certain faction within a certain range
        public static int findEntityWithinRange(Vector2 position, float radius, Faction factionToMatch, int entityToSkip = -1)
        {
            return findEntityWithinRange(position, radius, new List<Faction>(new[] { factionToMatch }), entityToSkip);
        }

        public static int findEntityWithinRange(Vector2 position, float radius, List<Faction> factionsToMatch, int entityToSkip = -1)
        {
            List<int> results = findEntitiesWithinRange(position, radius, factionsToMatch, entityToSkip);

            return results.Count > 0 ? results[0] : -1;
        }

        // Find entities of a certain faction along a ray
        public static List<int> findEntitiesAlongRay(Vector2 start, Vector2 normal, float length, Faction factionToMatch, int entityToSkip = -1)
        {
            return findEntitiesAlongRay(start, normal, length, new List<Faction>(new[] { factionToMatch }), entityToSkip);
        }

        public static List<int> findEntitiesAlongRay(Vector2 start, Vector2 normal, float length, List<Faction> factionsToMatch, int entityToSkip = -1)
        {
            List<int> results = new List<int>();

            SystemManager.physicsSystem.world.RayCast((f, p, n, fr) =>
                {
                    FactionComponent targetFactionComponent;
                    bool foundMatchingFaction = false;
                    int entityId;

                    // Skip if fixture body has no userdata
                    if (f.Body.UserData == null)
                    {
                        return -1;
                    }

                    entityId = (int)f.Body.UserData;

                    // Skip entity if entityToSkip was supplied and matched
                    if (entityToSkip != -1 && entityToSkip == entityId)
                    {
                        return -1;
                    }

                    // Skip if already in results list
                    if (results.Contains(entityId))
                    {
                        return -1;
                    }

                    // Skip if entity doesn't have a faction component
                    if ((targetFactionComponent = EntityManager.getFactionComponent(entityId)) == null)
                    {
                        return -1;
                    }

                    // Check factions
                    foreach (Faction factionToMatch in factionsToMatch)
                    {
                        if (targetFactionComponent.faction == factionToMatch)
                        {
                            foundMatchingFaction = true;
                            break;
                        }
                    }

                    // Skip target entity id if no matching factions found
                    if (!foundMatchingFaction)
                    {
                        return -1;
                    }

                    // Store entity id
                    results.Add(entityId);
                    return fr;
                },
                start,
                normal * length + start);

            return results;
        }

        // Mod
        public static int mod(int x, int modulus)
        {
            int result = x % modulus;
            return result < 0 ? modulus + x : result;
        }

        // Interpolate (cubic?)
        public static void interpolate(ref Vector2 a, ref Vector2 b, ref Vector2 c, ref Vector2 d, float mu, out Vector2 result)
        {
            float mu2 = mu * mu;
            float a0 = d.Y - c.Y - a.Y + b.Y;
            float a1 = a.Y - b.Y - a0;
            float a2 = c.Y - a.Y;
            float a3 = b.Y;
            result.Y = a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3;

            a0 = d.X - c.X - a.X + b.X;
            a1 = a.X - b.X - a0;
            a2 = c.X - a.X;
            a3 = b.X;
            result.X = a0 * mu * mu2 + a1 * mu2 + a2 * mu + a3;
        }

        // Bezier curve
        public static void bezier(ref Vector2 p0, ref Vector2 p1, ref Vector2 p2, ref Vector2 p3, float t, out Vector2 result)
        {
            float cx = 3 * (p1.X - p0.X);
            float cy = 3 * (p1.Y - p0.Y);
            float bx = 3 * (p2.X - p1.X) - cx;
            float by = 3 * (p2.Y - p1.Y) - cy;
            float ax = p3.X - p0.X - cx - bx;
            float ay = p3.Y - p0.Y - cy - by;
            float cube = t * t * t;
            float square = t * t;
            result.X = (ax * cube) + (bx * square) + (cx * t) + p0.X;
            result.Y = (ay * cube) + (by * square) + (cy * t) + p0.Y;
        }

        // Is polygon clockwise
        public static bool isPolygonClockwise(List<Vector2> points)
        {
            float sum = 0;

            for (int i = 0; i < points.Count; i++)
            {
                int j = i == points.Count - 1 ? 0 : i + 1;
                sum += (points[j].X - points[i].X) * (points[j].Y + points[i].Y);
            }

            return sum >= 0;
        }
    }
}
