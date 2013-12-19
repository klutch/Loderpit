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
        Explosion,
        Proxy,
        BattleDrone,
        ParticleRender,
        CharacterAnimation
    }

    public interface ISystem
    {
        SystemType systemType { get; }
        void update();
    }
}
