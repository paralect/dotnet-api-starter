using System.Threading.Tasks;
using Api.Controllers;
using Api.Core.Services.Interfaces.Document;
using Api.Models.User;
using AutoMapper;
using Common.DB.Postgres.DAL.Documents;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userService;
        private readonly Mock<IMapper> _mapper;

        public UsersControllerTests()
        {
            _userService = new Mock<IUserService>();
            _mapper = new Mock<IMapper>();
        }

        [Fact]
        public async Task GetCurrentShouldReturnOkObjectResult()
        {
            // Arrange
            var currentUserId = 10;// "test user id";
            var controller = CreateInstance(currentUserId);

            _userService.Setup(service => service.FindByIdAsync(currentUserId))
                .ReturnsAsync(new User());

            // Act
            var result = await controller.GetCurrentAsync();

            // Assert
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task UpdateCurrentShouldReturnBadRequestWhenCurrentUserIdIsNullOrEmpty()
        {
            // Arrange
            var controller = CreateInstance();

            // Act
            var result = await controller.UpdateCurrentAsync(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateCurrentShouldReturnBadRequestWhenEmailIsInUse()
        {
            // Arrange
            var currentUserId = 10;// "test id";
            var controller = CreateInstance(currentUserId);
            var model = new UpdateCurrentModel
            {
                Email = "test@test.test"
            };

            _userService.Setup(service => service.IsEmailInUseAsync(currentUserId, model.Email))
                .ReturnsAsync(true);

            // Act
            var result = await controller.UpdateCurrentAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task UpdateCurrentShouldUpdateInfoAndReturnOkObjectResult()
        {
            // Arrange
            var currentUserId = 10;// "test id";
            var controller = CreateInstance(currentUserId);
            var model = new UpdateCurrentModel
            {
                Email = "test@test.test",
                FirstName = "Test",
                LastName = "Tester"
            };

            _userService.Setup(service => service.IsEmailInUseAsync(currentUserId, model.Email))
                .ReturnsAsync(false);

            _userService.Setup(service => service.UpdateInfoAsync(currentUserId, model.Email, model.FirstName, model.LastName));

            _userService.Setup(service => service.FindByIdAsync(currentUserId))
                .ReturnsAsync(new User());

            // Act
            var result = await controller.UpdateCurrentAsync(model);

            // Assert
            _userService.Verify(service => service.UpdateInfoAsync(currentUserId, model.Email, model.FirstName, model.LastName));
            Assert.IsType<OkObjectResult>(result);
        }

        private UsersController CreateInstance(long? currentUserId = null)
        {
            var instance = new UsersController(_userService.Object, _mapper.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.User.Identity.Name).Returns(currentUserId?.ToString());

            var controllerContext = new ControllerContext { HttpContext = httpContext.Object };
            instance.ControllerContext = controllerContext;

            return instance;
        }
    }
}
