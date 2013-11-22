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
        EnemyAI
    }

    public interface ISystem
    {
        SystemType systemType { get; }
        void update();
    }
}
