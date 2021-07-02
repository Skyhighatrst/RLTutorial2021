using System;
using Components;
using Data;
using Unity.Entities;
using Unity.Transforms;

namespace Systems
{
    public class TryMoveSystem : GameSystemBase
    {
        protected override void OnUpdate()
        {
            var ecb = Barrier.CreateCommandBuffer().AsParallelWriter();

            Entities.WithAll<TryMove>().ForEach((Entity e, int entityInQueryIndex, ref Translation translation, in TryMove tryMove) =>
            {
                // For now, all moves will be valid, but in the future will have to check that the target location is valid
                // and doesn't contain another entity that cannot share the spot.

                var x = 0;
                var z = 0;

                switch (tryMove.Direction)
                {
                    case Direction.Northwest:
                        x = -1;
                        z = 1;
                        break;
                    case Direction.North:
                        z = 1;
                        break;
                    case Direction.Northeast:
                        x = 1;
                        z = 1;
                        break;
                    case Direction.East:
                        x = 1;
                        break;
                    case Direction.Southeast:
                        x = 1;
                        z = -1;
                        break;
                    case Direction.South:
                        z = -1;
                        break;
                    case Direction.Southwest:
                        x = -1;
                        z = -1;
                        break;
                    case Direction.West:
                        x = -1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }
                
                translation.Value.x += x;
                translation.Value.z += z;
                
                // Once the move is processed remove the component, so it will only be processed a single time.
                ecb.RemoveComponent<TryMove>(entityInQueryIndex, e);
                
                // Add the end turn component to signal to the EndTurnSystem that the
                // turn can be passed to the next entity or entities
                ecb.AddComponent<EndTurn>(entityInQueryIndex, e);
                ecb.RemoveComponent<TakingTurn>(entityInQueryIndex, e);
            }).ScheduleParallel();

            Dependency.Complete();
        }
    }
}