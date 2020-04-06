using Api.Controllers;
using Api.Core.DAL.Documents.User;
using Api.Core.Interfaces.Services.Document;
using Api.Models.User;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace Tests
{
    public class UsersControllerTests
    {
        private readonly Mock<IUserService> _userService;

        public UsersControllerTests()
        {
            _userService = new Mock<IUserService>();
        }

        [Fact]
        public async void GetCurrentShouldReturnOkObjectResult()
        {
            // Arrange
            var currentUserId = "test user id";
            var controller = CreateInstance(currentUserId);

            _userService.Setup(service => service.FindByIdAsync(currentUserId))
                .ReturnsAsync(new User());

            // Act
            var result = await controller.GetCurrentAsync();

            // Assert
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        [Fact]
        public async void UpdateCurrentShouldReturnBadRequestWhenCurrentUserIdIsNullOrEmpty()
        {
            // Arrange
            var controller = CreateInstance();

            // Act
            var result = await controller.UpdateCurrentAsync(null);

            // Assert
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async void UpdateCurrentShouldReturnBadRequestWhenEmailIsInUse()
        {
            // Arrange
            var currentUserId = "test id";
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
            Assert.IsAssignableFrom<BadRequestResult>(result);
        }

        [Fact]
        public async void UpdateCurrentShouldUpdateInfoAndReturnOkObjectResult()
        {
            // Arrange
            var currentUserId = "test id";
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
            Assert.IsAssignableFrom<OkObjectResult>(result);
        }

        private UsersController CreateInstance(string currentUserId = null)
        {
            var instance = new UsersController(_userService.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.User.Identity.Name).Returns(currentUserId);

            var controllerContext = new ControllerContext { HttpContext = httpContext.Object };
            instance.ControllerContext = controllerContext;

            return instance;
        }
    }
}
