using Unity.Entities;
using Data;

namespace Components
{
    /// <summary>
    /// Component that represents and attempt at moving in the given direction.
    /// </summary>
    public struct TryMove : IComponentData
    {
        public Direction Direction;
    }
}