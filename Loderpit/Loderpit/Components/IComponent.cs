using System;

namespace Loderpit.Components
{
    public enum ComponentType
    {
        Character,
        GroundBody,
        Rope,
        Bridge,
        RopeGrab,
        RopeGrabExclusion,
        IgnoreRopeRaycast,
        IgnoreBridgeRaycast,
        PositionTarget,
        DestructibleObstacle,
        Stats,
        Position,
        Ceiling,
        SendActivateObstacle,
        ReceiveActivateObstacleFall,
        Group,
        Skills,
        PerformingSkills,
        CombatTarget,
        Faction,
        Incapacitated,
        IsTouchingEndLevel,
        RenderHealth,
        ExternalMovementSpeeds,
        Shield,
        TrackEntityPosition,
        AffectedEntities,
        TimeToLive,
        StatModifier,
        DamageOverTime,
        DamageShield,
        AreaOfEffect,
        AffectedBySpellEntities,
        Proc,
        TimedExplosion,
        Physics,
        Dispellable,
        SlowMotion,
        DamageTransfer,
        Riposte,
        SpellType,
        DamageMitigation,
        AI
    }

    public interface IComponent
    {
        ComponentType componentType { get; }
        int entityId { get; }
    }
}
