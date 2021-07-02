using System;
using Components;
using Data;
using Unity.Collections;
using Unity.Entities;
using UnityEditor.SearchService;
using Random = Unity.Mathematics.Random;

namespace Systems
{
    public class NpcTurnSystem : GameSystemBase
    {
        private Random rand;
        private EntityQuery entitiesTakingTurns;

        protected override void OnCreate()
        {
            base.OnCreate();

            rand = new Random((uint) UnityEngine.Random.Range(1, Int32.MaxValue));
            entitiesTakingTurns = GetEntityQuery(
                ComponentType.ReadOnly<TakingTurn>(), 
                ComponentType.Exclude<PlayerTag>()
            );
        }

        protected override void OnUpdate()
        {
            var ecb = Barrier.CreateCommandBuffer().AsParallelWriter();

            var turnCount = entitiesTakingTurns.CalculateEntityCount();
            var directions = new NativeArray<Direction>(turnCount, Allocator.TempJob);
            for (var i = 0; i < turnCount; i++)
            {
                directions[i] = GetRandomDirection();
            }

            Entities.WithAll<TakingTurn>().WithNone<PlayerTag>().ForEach((Entity e, int entityInQueryIndex) =>
            {
                // ReSharper disable once AccessToDisposedClosure
                ecb.AddComponent(entityInQueryIndex, e, new TryMove { Direction = directions[entityInQueryIndex]});
            }).ScheduleParallel();

            Dependency.Complete();

            directions.Dispose();
        }

        private Direction GetRandomDirection()
        {
            var directions = new Direction[]
            {
                Direction.Northwest,
                Direction.North,
                Direction.Northeast,
                Direction.West,
                Direction.East,
                Direction.Southwest,
                Direction.Southeast,
                Direction.South
            };

            return directions[rand.NextInt(0, 7)];
        }
    }
}