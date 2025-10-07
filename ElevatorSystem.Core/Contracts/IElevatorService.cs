using ElevatorSystem.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Core.Contracts
{
    /// <summary>
    /// Defines a contract for elevator system operations.
    /// </summary>
    public interface IElevatorService
    {
        /// <summary>
        /// Gets the current state of all elevators managed by the service.
        /// </summary>
        IEnumerable<Elevator> GetElevators();

        /// <summary>
        /// Adds a randomly generated request to the pending queue.
        /// </summary>
        /// <param name="maxFloor">Maximum floor number allowed in the building.</param>
        void GenerateRandomRequest(int maxFloor);

        /// <summary>
        /// Adds a user-initiated floor request to the pending queue.
        /// </summary>
        /// <param name="request">The floor request from a user.</param>
        void AddUserRequest(FloorRequest request);

        /// <summary>
        /// Advances the simulation for all elevators by one step.
        /// Processes pending requests and moves elevators.
        /// </summary>
        Task StepAllAsync();
    }
}
