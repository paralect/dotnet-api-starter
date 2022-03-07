﻿using System.Threading.Tasks;
using Api.Controllers;
using Api.Services.Document;
using AutoMapper;
using Common.DAL.Documents.User;
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
            var currentUserId = "test user id";
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

        private UsersController CreateInstance(string currentUserId = null)
        {
            var instance = new UsersController(_userService.Object, _mapper.Object);

            var httpContext = new Mock<HttpContext>();
            httpContext.Setup(context => context.User.Identity.Name).Returns(currentUserId);

            var controllerContext = new ControllerContext { HttpContext = httpContext.Object };
            instance.ControllerContext = controllerContext;

            return instance;
        }
    }
}
