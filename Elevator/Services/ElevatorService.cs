using ElevatorApi.Models;
using Microsoft.Extensions.Logging;

namespace ElevatorApi.Services
{
    /// <summary>
    /// Manages a collection of elevators and handles floor requests, assignment, and elevator movement.
    /// </summary>
    public class ElevatorService
    {
        private readonly List<Elevator> _elevators = new List<Elevator>();
        private readonly List<FloorRequest> _requests = new List<FloorRequest>();
        private readonly object _lock = new object();
        private readonly Random _random = new Random();
        private readonly ILogger<ElevatorService> _logger;

        /// <summary>
        /// Initializes the ElevatorService with the specified number of elevators.
        /// </summary>
        /// <param name="numElevators">The number of elevators to manage.</param>
        /// <param name="logger">The logger instance.</param>
        public ElevatorService(int numElevators, ILogger<ElevatorService> logger)
        {
            _logger = logger;
            for (int i = 1; i <= numElevators; i++)
                _elevators.Add(new Elevator(i));
        }

        /// <summary>
        /// Gets the list/detail of elevators managed by this service.
        /// </summary>
        public IEnumerable<Elevator> GetElevators() => _elevators;

        /// <summary>
        /// Generates a random floor request and adds it to the pending requests list.
        /// </summary>
        /// <param name="maxFloor">The highest floor number in the building.</param>
        public void GenerateRandomRequest(int maxFloor)
        {
            int floor = _random.Next(1, maxFloor + 1);
            Direction dir = floor == maxFloor ? Direction.Down :
                            floor == 1 ? Direction.Up :
                            (_random.Next(0, 2) == 0 ? Direction.Up : Direction.Down);
            lock (_lock)
            {
                _requests.Add(new FloorRequest { Floor = floor, Direction = dir });
                _logger.LogInformation("Random request generated: {Direction} at floor {Floor}", dir, floor);
            }
        }

        /// <summary>
        /// Adds a user-generated floor request to the pending requests list.
        /// </summary>
        /// <param name="request">The floor request from a user.</param>
        public void AddUserRequest(FloorRequest request)
        {
            lock (_lock)
            {
                _requests.Add(request);
                _logger.LogInformation("User request received: {Direction} at floor {Floor}", request.Direction, request.Floor);
            }
        }

        /// <summary>
        /// Assigns pending floor requests to the most suitable elevators based on proximity and direction.
        /// </summary>
        private void AssignRequests()
        {
            lock (_lock)
            {
                foreach (var request in _requests.ToList())
                {
                    // Find an elevator that is idle or moving in the same direction as the request and is closest to the requested floor.
                    var candidate = _elevators
                        .Where(e => e.IsIdle || e.CurrentDirection == request.Direction)
                        .OrderBy(e => Math.Abs(e.CurrentFloor - request.Floor))
                        .FirstOrDefault();

                    if (candidate != null)
                    {
                        candidate.AddDestination(request.Floor);
                        _requests.Remove(request);
                        _logger.LogInformation("Assigned floor {Floor} to Elevator {ElevatorId}", request.Floor, candidate.Id);
                    }
                }
            }
        }

        /// <summary>
        /// Advances all elevators by one step and processes any pending requests.
        /// </summary>
        public async Task StepAllAsync()
        {
            AssignRequests();

            var tasks = _elevators.Select(async e =>
            {
                int oldFloor = e.CurrentFloor;
                await e.StepAsync();
            });

            await Task.WhenAll(tasks);
        }
    }
}