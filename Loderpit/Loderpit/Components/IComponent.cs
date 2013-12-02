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
        ActiveSpellEffects,
        ExternalMovementSpeeds,
        Shield
    }

    public interface IComponent
    {
        ComponentType componentType { get; }
        int entityId { get; }
    }
}
