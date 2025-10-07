using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ElevatorSystem.Core.Contracts;

namespace ElevatorSystem.Core.Services
{
    public class SimulationBackgroundService: BackgroundService, ISimulationBackgroundService
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<SimulationBackgroundService> _logger;
        private readonly int _maxFloors;
        private readonly Random _random = new Random();

        public SimulationBackgroundService(
            IElevatorService elevatorService,
            ILogger<SimulationBackgroundService> logger,
            IConfiguration configuration)
        {
            _elevatorService = elevatorService;
            _logger = logger;
            _maxFloors = 10;//configuration.GetValue<int>("ElevatorSettings:MaxFloor");
        }

        /// <summary>
        /// Executes the background simulation loop, generating random requests and stepping elevators.
        /// </summary>
        /// <param name="stoppingToken">Token to signal cancellation of the background task.</param>
        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            int simulationStepDelayMs = 10000;
            _logger.LogInformation("Simulation background service started.");
            while (!stoppingToken.IsCancellationRequested)
            {
                _elevatorService.GenerateRandomRequest(_maxFloors);
                // Advance the state of all elevators (move, stop, etc.).
                await _elevatorService.StepAllAsync();
                await Task.Delay(simulationStepDelayMs, stoppingToken);
            }
            _logger.LogInformation("Simulation background service stopped.");
        }

        public async Task RunSimulationAsync(CancellationToken stoppingToken)
        {
            int simulationStepDelayMs = 10000;
            _logger.LogInformation("Simulation background service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    if (_random.NextDouble() < 0.6)
                    {
                        _elevatorService.GenerateRandomRequest(_maxFloors);
                    }

                    await _elevatorService.StepAllAsync();
                    await Task.Delay(simulationStepDelayMs, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break; // Graceful shutdown
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred in simulation loop.");
                }
            }

            _logger.LogInformation("Simulation background service stopped.");
        }
    }
}
