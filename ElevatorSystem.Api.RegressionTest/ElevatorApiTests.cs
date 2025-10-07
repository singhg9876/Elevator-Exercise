using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using ElevatorSystem.Api.Controllers;
using ElevatorSystem.Core.Contracts;
using ElevatorSystem.Core.Models;
using ElevatorSystem.Core.Enum;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Microsoft.Extensions.Primitives;
using System.Linq;

namespace ElevatorSystem.Api.RegressionTest
{
    public class TestConfigurationSection : IConfigurationSection
    {
        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public string Key => "MaxFloor";
        public string Path => "ElevatorSettings:MaxFloor";
        public string Value { get; set; } = "10";
        public IEnumerable<IConfigurationSection> GetChildren() => Enumerable.Empty<IConfigurationSection>();
        public IChangeToken GetReloadToken() => new CancellationChangeToken(new CancellationToken());
        public IConfigurationSection GetSection(string key) => this;
    }

    public class TestConfiguration : IConfiguration
    {
        public string this[string key] { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public IEnumerable<IConfigurationSection> GetChildren() => Enumerable.Empty<IConfigurationSection>();
        public IChangeToken GetReloadToken() => new CancellationChangeToken(new CancellationToken());
        public IConfigurationSection GetSection(string key)
        {
            if (key == "ElevatorSettings:MaxFloor" || key == "ElevatorSettings")
                return new TestConfigurationSection();
            throw new NotImplementedException();
        }
        public T GetValue<T>(string key)
        {
            if (key == "ElevatorSettings:MaxFloor" && typeof(T) == typeof(int))
                return (T)(object)10;
            throw new NotSupportedException();
        }
    }

    [TestClass]
    public sealed class ElevatorApiTests
    {
        private Mock<IElevatorService> _serviceMock = null!;
        private Mock<ILogger<ElevatorController>> _loggerMock = null!;
        private IConfiguration _config = null!;
        private ElevatorController _controller = null!;

        [TestInitialize]
        public void Setup()
        {
            _serviceMock = new Mock<IElevatorService>();
            _loggerMock = new Mock<ILogger<ElevatorController>>();
            _config = new TestConfiguration();

            _controller = new ElevatorController(_serviceMock.Object, _loggerMock.Object, _config);
        }

        [TestMethod]
        public void GetElevators_ReturnsElevatorList()
        {
            var elevators = new List<Elevator>
            {
                new Elevator(1),
                new Elevator(2)
            };
            _serviceMock.Setup(s => s.GetElevators()).Returns(elevators);

            var result = _controller.GetElevators();

            Assert.IsNotNull(result);
            CollectionAssert.AreEqual(elevators, new List<Elevator>(result));
        }

        [TestMethod]
        public void CreateRequest_ValidRequest_ReturnsOk()
        {
            var request = new FloorRequest { Floor = 5, Direction = Direction.Up };

            var result = _controller.CreateRequest(request);

            Assert.IsInstanceOfType(result, typeof(OkObjectResult));
            var okResult = (OkObjectResult)result;
            StringAssert.Contains(okResult.Value.ToString(), "Request added");
            _serviceMock.Verify(s => s.AddUserRequest(request), Times.Once);
        }

        [TestMethod]
        public void CreateRequest_InvalidFloor_ReturnsBadRequest()
        {
            var request = new FloorRequest { Floor = 0, Direction = Direction.Up };

            var result = _controller.CreateRequest(request);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badResult = (BadRequestObjectResult)result;
            StringAssert.Contains(badResult.Value.ToString(), "Floor must be between 1 and 10");
        }

        [TestMethod]
        public void CreateRequest_InvalidDirection_ReturnsBadRequest()
        {
            var request = new FloorRequest { Floor = 5, Direction = Direction.Idle };

            var result = _controller.CreateRequest(request);

            Assert.IsInstanceOfType(result, typeof(BadRequestObjectResult));
            var badResult = (BadRequestObjectResult)result;
            StringAssert.Contains(badResult.Value.ToString(), "Direction must be Up or Down");
        }

        [TestMethod]
        public void CreateRequest_EdgeFloorValues_Accepted()
        {
            var requestLow = new FloorRequest { Floor = 1, Direction = Direction.Up };
            var requestHigh = new FloorRequest { Floor = 10, Direction = Direction.Down };

            var resultLow = _controller.CreateRequest(requestLow);
            var resultHigh = _controller.CreateRequest(requestHigh);

            Assert.IsInstanceOfType(resultLow, typeof(OkObjectResult));
            Assert.IsInstanceOfType(resultHigh, typeof(OkObjectResult));
        }
    }
}
