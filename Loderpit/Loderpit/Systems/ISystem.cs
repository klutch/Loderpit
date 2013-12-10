using System;

namespace Loderpit.Systems
{
    public enum SystemType
    {
        Camera,
        Character,
        Physics,
        Team,
        Stat,
        Render,
        Obstacle,
        Group,
        Skill,
        Combat,
        Level,
        AI,
        InterLevel,
        Spell,
        Explosion
    }

    public interface ISystem
    {
        SystemType systemType { get; }
        void update();
    }
}
