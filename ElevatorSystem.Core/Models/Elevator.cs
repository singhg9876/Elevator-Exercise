using ElevatorSystem.Core.Enum;
using ElevatorSystem.Core.State;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Core.Models
{
    public class Elevator
    {
        /// <summary>
        /// Unique identifier for the elevator.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Indicates whether the elevator is idle (has no destinations).
        /// </summary>
        public bool IsIdle => DestinationManager.IsIdle && CurrentDirection == Direction.Idle;

        /// <summary>
        /// The current floor where the elevator is located.
        /// </summary>
        public int CurrentFloor { get; private set; } = 1;

        /// <summary>
        /// The current direction of the elevator (Up, Down, or Idle).
        /// </summary>
        public Direction CurrentDirection { get; private set; } = Direction.Idle;

        private ElevatorDestinationManager DestinationManager;

        /// <summary>
        /// Queue of destination floors for the elevator to visit.
        /// </summary>
        // Expose the actual destination queue from the manager
        public Queue<int> Destinations => new Queue<int>(DestinationManager.Destinations);

        public int DestinationCount => DestinationManager.Destinations.Count;

        /// <summary>
        /// Initializes a new instance of the Elevator class with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier for the elevator.</param>
        public Elevator(int id)
        {
            Id = id;
            DestinationManager = new ElevatorDestinationManager();
        }

        /// <summary>
        /// Adds a floor to the destination queue if it is not already present.
        /// </summary>
        /// <param name="floor">The floor to add as a destination.</param>
        public void AddDestination(int floor)
        {
            DestinationManager.AddDestination(floor);
        }


        /// <summary>
        /// Advances the elevator by one step: moves towards the next destination or stops if at the destination.
        /// </summary>
        public async Task Step()
        {
            if (DestinationManager.IsIdle)
            {
                CurrentDirection = Direction.Idle;
                return;
            }

            int targetFloor = DestinationManager.PeekNextDestination();
            if (CurrentFloor == targetFloor)
            {
                DestinationManager.RemoveCurrentDestination();
                // Set direction to Idle if no more destinations
                if (DestinationManager.IsIdle)
                {
                    CurrentDirection = Direction.Idle;
                }
            }
            else
            {
                CurrentDirection = CurrentFloor < targetFloor ? Direction.Up : Direction.Down;
                CurrentFloor += CurrentDirection == Direction.Up ? 1 : -1;
            }
            // If after step, no destinations remain, ensure direction is Idle
            if (DestinationManager.IsIdle)
            {
                CurrentDirection = Direction.Idle;
            }
        }
    }
}
