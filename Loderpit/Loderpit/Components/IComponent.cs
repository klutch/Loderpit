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
        CreateRope,
        CreateBridge,
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
        CombatTarget,
        Faction,
        Incapacitated,
        IsTouchingEndLevel,
        RenderHealth
    }

    public interface IComponent
    {
        ComponentType componentType { get; }
        int entityId { get; }
    }
}
