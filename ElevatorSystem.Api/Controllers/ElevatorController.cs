using ElevatorSystem.Core.Contracts;
using ElevatorSystem.Core.Enum;
using ElevatorSystem.Core.Models;
using ElevatorSystem.Core.Services;
using Microsoft.AspNetCore.Mvc;

namespace ElevatorSystem.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ElevatorController : Controller
    {
        private readonly IElevatorService _elevatorService;
        private readonly ILogger<ElevatorController> _logger;
        private readonly int _maxFloors;

        public ElevatorController(IElevatorService elevatorService, ILogger<ElevatorController> logger, IConfiguration configuration)
        {
            _elevatorService = elevatorService;
            _logger = logger;
            _maxFloors = configuration.GetValue<int>("ElevatorSettings:MaxFloor");
        }

        /// <summary>
        /// Gets the current status of all elevators.
        /// </summary>
        [HttpGet]
        public IEnumerable<Elevator> GetElevators()
        {
            _logger.LogInformation("Fetching elevator statuses.");
            return _elevatorService.GetElevators();
        }

        /// <summary>
        /// Creates a new floor request for an elevator.
        /// </summary>
        /// <param name="request">The floor request containing the floor and direction.</param>
        /// <returns>Result of the request.</returns>
        [HttpPost("request")]
        public IActionResult CreateRequest([FromBody] FloorRequest request)
        {
            if (request.Floor < 1 || request.Floor > _maxFloors)
            {
                _logger.LogWarning("Invalid floor request: {Floor}", request.Floor);
                return BadRequest("Floor must be between 1 and 10.");
            }

            if (request.Direction != Direction.Up && request.Direction != Direction.Down)
            {
                _logger.LogWarning("Invalid direction request: {Direction}", request.Direction);
                return BadRequest("Direction must be Up or Down.");
            }

            _elevatorService.AddUserRequest(request);
            _logger.LogInformation("Floor request created: {Direction} at floor {Floor}", request.Direction, request.Floor);

            return Ok($"Request added: {request.Direction} at floor {request.Floor}");
        }

    }
}
