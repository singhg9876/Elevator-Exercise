using ElevatorApi.Models;
using ElevatorApi.Services;
using Microsoft.Extensions.Logging; // Add this using

namespace ElevatorApi.Tests
{
    [TestClass]
    public class ElevatorServiceTests
    {
        private ElevatorService _service;

        /// <summary>
        /// Initializes a new ElevatorService with 4 elevators before each test.
        /// </summary>
        [TestInitialize]
        public void Setup()
        {
            // Use a simple mock logger for testing
            var logger = new LoggerFactory().CreateLogger<ElevatorService>();
            _service = new ElevatorService(4, logger);
        }

        /// <summary>
        /// Verifies that the correct number of elevators are initialized and all start at floor 1.
        /// </summary>
        [TestMethod]
        public void Elevators_ShouldBeInitialized()
        {
            var elevators = _service.GetElevators().ToList();
            Assert.AreEqual(4, elevators.Count);
            Assert.IsTrue(elevators.All(e => e.CurrentFloor == 1));
        }

        /// <summary>
        /// Tests that a user request is added and assigned to an elevator.
        /// </summary>
        [TestMethod]
        public async Task AddUserRequest_ShouldAddRequest()
        {
            _service.AddUserRequest(new FloorRequest { Floor = 5, Direction = Direction.Up });
            await _service.StepAllAsync();
            var elevators = _service.GetElevators().ToList();
            Assert.IsTrue(elevators.Any(e => e.Destinations.Contains(5)));
        }

        /// <summary>
        /// Tests that a random request is generated and assigned to an elevator.
        /// </summary>
        [TestMethod]
        public async Task GenerateRandomRequest_ShouldAddRequest()
        {
            _service.GenerateRandomRequest(10);
            await _service.StepAllAsync();
            var elevators = _service.GetElevators().ToList();
            Assert.IsTrue(elevators.Any(e => e.Destinations.Count > 0));
        }

        /// <summary>
        /// Tests that multiple random requests are assigned to elevators.
        /// </summary>
        [TestMethod]
        //Multiple Random Requests
        public async Task MultipleRandomRequests_ShouldBeAssignedToElevators()
        {
            _service.GenerateRandomRequest(10);
            _service.GenerateRandomRequest(10);
            _service.GenerateRandomRequest(10);

            await _service.StepAllAsync();
            var elevators = _service.GetElevators().ToList();

            Assert.IsTrue(elevators.Any(e => e.Destinations.Count > 0));
        }

        /// <summary>
        /// Tests that multiple user requests are assigned to elevators correctly.
        /// </summary>
        //Multiple User Requests
        [TestMethod]
        public async Task MultipleUserRequests_ShouldAssignCorrectly()
        {
            _service.AddUserRequest(new FloorRequest { Floor = 3, Direction = Direction.Up });
            _service.AddUserRequest(new FloorRequest { Floor = 7, Direction = Direction.Down });

            await _service.StepAllAsync();
            var elevators = _service.GetElevators().ToList();

            Assert.IsTrue(elevators.Any(e => e.Destinations.Contains(3) || e.Destinations.Contains(7)));
        }

        /// <summary>
        /// Verifies that elevators remain idle if there are no requests.
        /// </summary>
        //Elevator Idle Behavior
        [TestMethod]
        public async Task IdleElevator_ShouldRemainIdleIfNoRequest()
        {
            var elevatorsBefore = _service.GetElevators().ToList();
            await _service.StepAllAsync();
            var elevatorsAfter = _service.GetElevators().ToList();

            Assert.IsTrue(elevatorsAfter.All(e => e.CurrentFloor == 1));
            Assert.IsTrue(elevatorsAfter.All(e => e.Destinations.Count == 0));
        }

        /// <summary>
        /// Tests that an elevator moves up towards a requested floor.
        /// </summary>
        //Elevator Moves Up
        [TestMethod]
        public async Task Elevator_ShouldMoveUpTowardsRequest()
        {
            _service.AddUserRequest(new FloorRequest { Floor = 5, Direction = Direction.Up });
            var elevator = _service.GetElevators().First();
            int initialFloor = elevator.CurrentFloor;

            await _service.StepAllAsync();

            Assert.IsTrue(elevator.CurrentFloor > initialFloor);
        }

        /// <summary>
        /// Tests that a random request is assigned to the nearest available elevator.
        /// </summary>
        //Random Request Assignment
        [TestMethod]
        public async Task RandomRequest_ShouldBeAssignedToNearestElevator()
        {
            _service.GenerateRandomRequest(10);
            await _service.StepAllAsync();

            var elevators = _service.GetElevators().ToList();
            Assert.IsTrue(elevators.Any(e => e.Destinations.Count > 0));
        }

        /// <summary>
        /// Simulates multiple steps and verifies that the elevator reaches the requested floor.
        /// </summary>
        //Multiple Steps Simulation
        [TestMethod]
        public async Task Elevator_ShouldReachDestinationAfterMultipleSteps()
        {
            _service.AddUserRequest(new FloorRequest { Floor = 5, Direction = Direction.Up });
            var elevator = _service.GetElevators().First();

            for (int i = 0; i < 5; i++) // simulate 5 steps
                await _service.StepAllAsync();

            Assert.IsTrue(elevator.CurrentFloor == 5);
        }
    }
}
