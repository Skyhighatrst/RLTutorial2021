using Components;
using Data;
using Unity.Entities;
using UnityEngine;

namespace Systems
{
    public class InputSystem : SystemBase
    {
        protected override void OnUpdate()
        {
            var direction = MovementInput();
            if (direction != null)
            {
                // There should only really ever be a limited number of player tagged entities
                // so it should be ok and not too great a performance hit to simply iterate and perform the 
                // structural changes directly. There really should only ever be a single entity with both the 
                // PlayerTag and PlayerTurn component.
                
                // Interesting to note, it appears as though, if the below query doesn't have any  entities,
                // then the system doesn't run at all.
                Entities
                    .WithAll<PlayerTag, TakingTurn>()
                    .WithoutBurst()
                    .WithStructuralChanges()
                    .ForEach((Entity e) =>
                    {
                        EntityManager.AddComponent<TryMove>(e);
                        EntityManager.AddComponentData(e, new TryMove {Direction = direction.Value});
                    }).Run();
            }
        }

        /// <summary>
        /// Map movement keys to a direction. Only supports keypad movement for the time being.
        /// TODO Consider using Unity's new input handlers for better key map definitions.
        /// </summary>
        /// <returns>The <see cref="Direction"/> that corresponds to the movement input</returns>
        private Direction? MovementInput()
        {
            if (Input.GetKeyDown(KeyCode.Keypad7)) return Direction.Northwest;
            if (Input.GetKeyDown(KeyCode.Keypad8)) return Direction.North;
            if (Input.GetKeyDown(KeyCode.Keypad9)) return Direction.Northeast;
            if (Input.GetKeyDown(KeyCode.Keypad6)) return Direction.East;
            if (Input.GetKeyDown(KeyCode.Keypad4)) return Direction.West;
            if (Input.GetKeyDown(KeyCode.Keypad1)) return Direction.Southwest;
            if (Input.GetKeyDown(KeyCode.Keypad2)) return Direction.South;
            if (Input.GetKeyDown(KeyCode.Keypad3)) return Direction.Southeast;

            return null;
        }
    }
}