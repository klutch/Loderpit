using System;

namespace Loderpit
{
    public class Helpers
    {
        private static float _pi = (float)Math.PI;

        public static float randomBetween(Random rng, float low, float high)
        {
            float rLow = Math.Min(low, high);
            float rHigh = Math.Max(low, high);
            float range = rHigh - rLow;
            return (float)rng.NextDouble() * range + rLow;
        }

        public static float degToRad(float degrees)
        {
            return degrees * (_pi / 180f);
        }

        public static float radToDeg(float radians)
        {
            return radians * (180f / _pi) - 90;
        }
    }
}
