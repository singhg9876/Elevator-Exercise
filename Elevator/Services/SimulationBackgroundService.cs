using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace ElevatorApi.Services
{
    /// <summary>
    /// Background service that simulates elevator activity by periodically generating random requests
    /// </summary>
    public class SimulationBackgroundService : BackgroundService
    {
        private readonly ElevatorService _elevatorService;
        private readonly ILogger<SimulationBackgroundService> _logger;
        private readonly int _maxFloors;
        private readonly Random _random = new Random();

        public SimulationBackgroundService(
            ElevatorService elevatorService,
            ILogger<SimulationBackgroundService> logger,
            IConfiguration configuration)
        {
            _elevatorService = elevatorService;
            _logger = logger;
            _maxFloors = configuration.GetValue<int>("ElevatorSettings:MaxFloor");
        }

        /// <summary>
        /// Executes the background simulation loop, generating random requests and stepping elevators.
        /// </summary>
        /// <param name="stoppingToken">Token to signal cancellation of the background task.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Simulation background service started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                // 30% chance to generate a random elevator call on each step.
                if (_random.NextDouble() < 0.3)
                {
                    _elevatorService.GenerateRandomRequest(_maxFloors);
                }

                // Advance the state of all elevators (move, stop, etc.).
                await _elevatorService.StepAllAsync();
            }
            _logger.LogInformation("Simulation background service stopped.");
        }
    }
}
