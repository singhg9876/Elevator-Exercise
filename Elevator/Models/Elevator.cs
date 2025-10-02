using System.Collections.Generic;
using System.Threading.Tasks;
using System;

namespace ElevatorApi.Models
{
    /// <summary>
    /// Represents a single elevator, tracking its state and handling movement between floors.
    /// </summary>
    public class Elevator
    {
        /// <summary>
        /// Unique identifier for the elevator.
        /// </summary>
        public int Id { get; }

        /// <summary>
        /// Indicates whether the elevator is idle (has no destinations).
        /// </summary>
        public bool IsIdle => Destinations.Count == 0;

        /// <summary>
        /// The current floor where the elevator is located.
        /// </summary>
        public int CurrentFloor { get; private set; } = 1;

        /// <summary>
        /// The current direction of the elevator (Up, Down, or Idle).
        /// </summary>
        public Direction CurrentDirection { get; private set; } = Direction.Idle;

        /// <summary>
        /// Queue of destination floors for the elevator to visit.
        /// </summary>
        public Queue<int> Destinations { get; } = new Queue<int>();

        // Time in seconds to move between floors.
        private const int MoveTimeSeconds = 10;

        // Time in seconds to stop at a floor for passengers.
        private const int StopTimeSeconds = 10;

        /// <summary>
        /// Initializes a new instance of the Elevator class with the specified ID.
        /// </summary>
        /// <param name="id">The unique identifier for the elevator.</param>
        public Elevator(int id)
        {
            Id = id;
        }

        /// <summary>
        /// Adds a floor to the destination queue if it is not already present.
        /// </summary>
        /// <param name="floor">The floor to add as a destination.</param>
        public void AddDestination(int floor)
        {
            if (!Destinations.Contains(floor))
            {
                Destinations.Enqueue(floor);
            }
        }

        /// <summary>
        /// Advances the elevator by one step: moves towards the next destination or stops if at the destination.
        /// Simulates time taken for moving and stopping using Task.Delay.
        /// </summary>
        public async Task StepAsync()
        {
            if (Destinations.Count == 0)
            {
                CurrentDirection = Direction.Idle;
                return;
            }

            int targetFloor = Destinations.Peek();
            if (CurrentFloor == targetFloor)
            {
                Console.WriteLine($"Elevator {Id} stopped at floor {CurrentFloor}.");
                Destinations.Dequeue();
                await Task.Delay(StopTimeSeconds * 1000); // Simulate stop time for passengers
            }
            else
            {
                CurrentDirection = CurrentFloor < targetFloor ? Direction.Up : Direction.Down;
                CurrentFloor += CurrentDirection == Direction.Up ? 1 : -1;
                Console.WriteLine($"Elevator {Id} moving {CurrentDirection} to floor {CurrentFloor}.");
                await Task.Delay(MoveTimeSeconds * 1000); // Simulate move time between floors
            }
        }
    }
}