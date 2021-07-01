using Unity.Entities;

namespace Components
{
    /// <summary>
    /// Tag that indicates that an entity is currently awaiting it's next turn.
    /// </summary>
    public struct AwaitingTurn : IComponentData { }
    
    /// <summary>
    /// Tag that indicates that an entity is currently taking it's turn.
    /// </summary>
    public struct TakingTurn : IComponentData { }
    
    /// <summary>
    /// Tag that indicates that an entity has just taken it's turn and the EndTurnSystem should process it.
    /// </summary>
    public struct EndTurn : IComponentData { }

    /// <summary>
    /// Component that tracks the amount of energy the entity has gained. Once it passes a threshold it will trigger
    /// the entity's next turn and be reset to 0.
    /// </summary>
    public struct Energy : IComponentData
    {
        public float Value;
    }

    /// <summary>
    /// Component that defines an entity's speed. Controls how quickly the entity may take turns by applying a multiplier
    /// to energy gained each turn.
    /// </summary>
    public struct Speed : IComponentData
    {
        public float Value;
    }
}