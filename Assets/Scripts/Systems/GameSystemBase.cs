using Unity.Entities;

namespace Systems
{
    public abstract class GameSystemBase : SystemBase
    {
        protected EndSimulationEntityCommandBufferSystem Barrier;

        protected override void OnCreate()
        {
            Barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
        }
    }
}