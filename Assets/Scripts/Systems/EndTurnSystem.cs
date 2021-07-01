using Components;
using Unity.Entities;
using Unity.Entities.CodeGeneratedJobForEach;
using UnityEngine;

namespace Systems
{
    /// <summary>
    /// System that process end turn events and determines which entity or entities should take their turn next.
    /// Has not yet been tested with non player entities, that come later.
    /// </summary>
    public class EndTurnSystem : SystemBase
    {
        private EndSimulationEntityCommandBufferSystem barrier;
        private EntityQuery entitiesEndingTurn;
        private EntityQuery entitiesTakingTurns;
        private const float Threshold = 10;

        protected override void OnCreate()
        {
            barrier = World.GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
            entitiesEndingTurn = GetEntityQuery(ComponentType.ReadOnly<EndTurn>());
            entitiesTakingTurns = GetEntityQuery(ComponentType.ReadOnly<TakingTurn>());
        }
        
        protected override void OnUpdate()
        {
            var ecb = barrier.CreateCommandBuffer().AsParallelWriter();

            // Turn count determines how many entities are ending their turn. 
            // if it's a player ending a turn this should only be one, but non player
            // entities can take their turns simultaneously.
            var turnCount = entitiesEndingTurn.CalculateEntityCount();
            var entitiesTakingTurnCount = entitiesTakingTurns.CalculateEntityCount();
            
            // We have nothing to do if there are no turns ending, and we're waiting for entities to take their turns
            if (turnCount == 0 && entitiesTakingTurnCount > 0)
            {
                return;
            }

            // There are no turns ending, and no turns in progress. We need to still hand out energy though so that
            // we can eventually get an entity with a turn.
            if (turnCount == 0 && entitiesTakingTurnCount == 0)
            {
                // take a single turn so that we can still hand out some energy
                turnCount = 1;
            }

            var playerHasNextTurn = false;
            // Process any player entities first. As soon as we find one that should take it's next turn, all future
            // player entities should get their energy, but not be assigned a next turn.
            Entities.WithAll<AwaitingTurn, Energy, Speed>().WithAll<PlayerTag>()
                .WithNone<TakingTurn>()
                .WithoutBurst()
                .WithStructuralChanges()
                .ForEach(
                (Entity e, ref Energy energy, in Speed speed) =>
                {
                    energy.Value += turnCount * speed.Value;
                    
                    if (energy.Value >= Threshold && !playerHasNextTurn)
                    {
                        playerHasNextTurn = true;
                        EntityManager.RemoveComponent<AwaitingTurn>(e);
                        EntityManager.AddComponent<TakingTurn>(e);
                    }
                }).Run();

            Dependency.Complete();
            
            // Hand out energy to any entities that are awaiting their turn. If they pass the energy threshold
            // then assign them their next turn. Only do this for non player entities, and only if there are no
            // player entities that should be taking their turn.
            Entities.WithAll<AwaitingTurn, Energy, Speed>().WithNone<PlayerTag, TakingTurn>()
                .ForEach(
                (Entity e, int entityInQueryIndex, ref Energy energy, in Speed speed) =>
                {
                    energy.Value += turnCount * speed.Value;
                    
                    if (energy.Value >= Threshold && !playerHasNextTurn)
                    {
                        ecb.RemoveComponent<AwaitingTurn>(entityInQueryIndex, e);
                        ecb.AddComponent<TakingTurn>(entityInQueryIndex, e);
                    }
                }).ScheduleParallel();

            Dependency.Complete();
            
            // Finally, remove the EndTurn tag from all entities and add the AwaitingTurn tag.
            Entities.WithAll<EndTurn, Energy>().ForEach((Entity e, int entityInQueryIndex, ref Energy energy) =>
            {
                energy.Value = 0;
                ecb.RemoveComponent<EndTurn>(entityInQueryIndex, e);
                ecb.AddComponent<AwaitingTurn>(entityInQueryIndex, e);
            }).ScheduleParallel();

            Dependency.Complete();
        }
    }
}