using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using ElevatorSystem.Core.Services;
using ElevatorSystem.Core.Models;
using ElevatorSystem.Core.Enum;
using System.Linq;
using System.Threading.Tasks;
using System;

namespace ElevatorSystem.Core.RegressionTests
{
    [TestClass]
    public class ElevatorCoreTests
    {
        private Mock<ILogger<ElevatorService>> _loggerMock = null!;
        private ElevatorService _service = null!;

        [TestInitialize]
        public void Setup()
        {
            _loggerMock = new Mock<ILogger<ElevatorService>>();
            var seededRandom = new Random(42);
            _service = new ElevatorService(numElevators: 3, _loggerMock.Object, seededRandom);
        }

        [TestMethod]
        public void Constructor_CreatesCorrectNumberOfElevators()
        {
            var elevators = _service.GetElevators().ToList();
            Assert.AreEqual(3, elevators.Count, "Number of elevators created should match constructor parameter.");
        }

        [TestMethod]
        public async Task AddUserRequest_AssignsRequestToSomeElevator()
        {
            var req = new FloorRequest { Floor = 5, Direction = Direction.Up };
            _service.AddUserRequest(req);
            await _service.StepAllAsync();
            var anyAssigned = _service.GetElevators().Any(e => e.DestinationCount > 0);
            Assert.IsTrue(anyAssigned, "No elevator received the user request.");
        }

        [TestMethod]
        public async Task MultipleRequests_AreDistributed_NotAllToOneElevator()
        {
            _service.AddUserRequest(new FloorRequest { Floor = 2, Direction = Direction.Up });
            _service.AddUserRequest(new FloorRequest { Floor = 7, Direction = Direction.Down });
            _service.AddUserRequest(new FloorRequest { Floor = 9, Direction = Direction.Down });
            await _service.StepAllAsync();
            var assignedCount = _service.GetElevators().Count(e => e.DestinationCount > 0);
            Assert.IsTrue(assignedCount >= 2, $"Expected requests to be distributed. Assigned elevators: {assignedCount}");
        }

        [TestMethod]
        public async Task Elevator_MovesUpAndDownCorrectly()
        {
            var elevator = _service.GetElevators().First();
            elevator.AddDestination(3);
            await elevator.Step();
            Assert.AreEqual(2, elevator.CurrentFloor);
            await elevator.Step();
            Assert.AreEqual(3, elevator.CurrentFloor);
            await elevator.Step();
            Assert.IsTrue(elevator.IsIdle || elevator.DestinationCount == 0);
        }

        [TestMethod]
        public async Task Elevator_BecomesIdleAfterAllDestinations()
        {
            var elevator = _service.GetElevators().First();
            elevator.AddDestination(2);
            await elevator.Step();
            await elevator.Step();
            Assert.IsTrue(elevator.IsIdle, "Elevator should be idle after reaching all destinations.");
        }

        [TestMethod]
        public void Elevator_IgnoresDuplicateDestinations()
        {
            var elevator = _service.GetElevators().First();
            elevator.AddDestination(5);
            elevator.AddDestination(5);
            Assert.AreEqual(1, elevator.DestinationCount, "Duplicate destinations should not be added.");
        }

        [TestMethod]
        public async Task Elevator_HandlesRequestForCurrentFloor()
        {
            var elevator = _service.GetElevators().First();
            elevator.AddDestination(1); // Already at floor 1
            await elevator.Step();
            Assert.IsTrue(elevator.IsIdle, "Elevator should be idle after handling request for current floor.");
        }

        [TestMethod]
        public async Task MultipleElevators_OperateIndependently()
        {
            var elevators = _service.GetElevators().ToList();
            elevators[0].AddDestination(2);
            elevators[1].AddDestination(3);
            await Task.WhenAll(elevators.Select(e => e.Step()));
            Assert.AreEqual(2, elevators[0].CurrentFloor);
            Assert.AreEqual(2, elevators[1].CurrentFloor);
        }

        [TestMethod]
        public async Task GenerateRandomRequest_AssignsRequest()
        {
            _service.GenerateRandomRequest(10);
            await _service.StepAllAsync();
            var anyAssigned = _service.GetElevators().Any(e => e.DestinationCount > 0);
            Assert.IsTrue(anyAssigned, "Random request was not assigned to any elevator.");
        }

        [TestMethod]
        public void AddUserRequest_HandlesEdgeFloorValues()
        {
            _service.AddUserRequest(new FloorRequest { Floor = 1, Direction = Direction.Up });
            _service.AddUserRequest(new FloorRequest { Floor = 10, Direction = Direction.Down });
            // No exception should be thrown, requests should be added
        }
    }
}
