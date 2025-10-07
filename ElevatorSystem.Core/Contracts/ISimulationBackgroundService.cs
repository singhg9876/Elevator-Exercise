using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ElevatorSystem.Core.Contracts
{
    public interface ISimulationBackgroundService
    {
        /// <summary>
        /// Starts the simulation loop asynchronously.
        /// </summary>
        /// <param name="stoppingToken">Token used to signal when the simulation should stop.</param>
        Task RunSimulationAsync(CancellationToken stoppingToken);
    }
}
