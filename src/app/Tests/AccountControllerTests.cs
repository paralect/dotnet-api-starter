using System.Threading.Tasks;
using Api.Controllers;
using Api.Core;
using Api.Core.Services.Document.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Document;
using Api.Core.Services.Interfaces.Infrastructure;
using Api.Core.Settings;
using Api.Core.Utils;
using Api.Models.Account;
using AutoMapper;
using Common.DAL.Documents.User;
using Common.DAL.Repositories;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Moq;
using Xunit;
using ForgotPasswordModel = Api.Models.Account.ForgotPasswordModel;

namespace Tests
{
    public class AccountControllerTests
    {
        private readonly Mock<IEmailService> _emailService;
        private readonly Mock<IUserService> _userService;
        private readonly Mock<ITokenService> _tokenService;
        private readonly Mock<IAuthService> _authService;
        private readonly Mock<IWebHostEnvironment> _environment;
        private readonly Mock<IOptions<AppSettings>> _appSettingsOptions;
        private readonly Mock<IGoogleService> _googleService;
        private readonly Mock<IMapper> _mapper;
        private readonly AppSettings _appSettings;

        public AccountControllerTests()
        {
            _emailService = new Mock<IEmailService>();
            _userService = new Mock<IUserService>();
            _tokenService = new Mock<ITokenService>();
            _authService = new Mock<IAuthService>();
            _environment = new Mock<IWebHostEnvironment>();
            _appSettingsOptions = new Mock<IOptions<AppSettings>>();
            _googleService = new Mock<IGoogleService>();
            _mapper = new Mock<IMapper>();

            _appSettings = new AppSettings
            {
                WebUrl = "http://test.com",
                LandingUrl = "http://test-landing.com"
            };
        }

        [Fact]
        public async Task SignUpShouldReturnBadRequestWhenUserAlreadyExist()
        {
            // Arrange
            var model = new SignUpModel
            {
                Email = "sample@sample.com"
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync(new User());

            
            var controller = CreateInstance();

            // Act
            var result = await controller.SignUpAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Theory]
        [InlineData("Development")]
        [InlineData("Production")]
        public async Task SignUpShouldReturnOkWhenUserDoesNotExist(string environmentName)
        {
            // Arrange
            var expectedResult = environmentName == "Development" ? typeof(OkObjectResult) : typeof(OkResult);
            var model = new SignUpModel
            {
                Email = "sample@sample.com"
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync((User)null);

            _userService.Setup(service => service.CreateUserAccountAsync(It.IsAny<CreateUserModel>()))
                .ReturnsAsync(new User());

            _environment.Setup(environment => environment.EnvironmentName).Returns(environmentName);

            var controller = CreateInstance();

            // Act
            var result = await controller.SignUpAsync(model);

            // Assert
            Assert.IsType(expectedResult, result);
        }

        [Fact]
        public async Task VerifyEmailShouldReturnBadRequestWhenTokenIsNull()
        {
            // Arrange
            var controller = CreateInstance();

            // Act
            var result = await controller.VerifyEmailAsync(null);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task VerifyEmailShouldReturnBadRequestWhenTokenIsInvalid()
        {
            // Arrange
            var token = "sample";

            _userService.Setup(service => service.FindOneAsync(It.Is<UserFilter>(filter => filter.SignUpToken == token)))
                .ReturnsAsync((User)null);
            
            var controller = CreateInstance();

            // Act
            var result = await controller.VerifyEmailAsync(token);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task VerifyEmailShouldMarkEmailAsVerifiedAndReturnRedirectToMainPage()
        {
            // Arrange
            var token = "sample";
            var userId = "user id sample";

            _userService.Setup(service => service.FindOneAsync(It.Is<UserFilter>(filter => filter.SignUpToken == token)))
                .ReturnsAsync(new User { Id = userId });
            
            var controller = CreateInstance();

            // Act
            var result = await controller.VerifyEmailAsync(token);

            // Assert
            _userService.Verify(service => service.MarkEmailAsVerifiedAsync(userId), Times.Once);
            _userService.Verify(service => service.UpdateLastRequestAsync(userId), Times.Once);
            _authService.Verify(service => service.SetTokensAsync(userId), Times.Once);
            Assert.True((result as RedirectResult)?.Url == _appSettings.WebUrl);
        }

        [Fact]
        public async Task SignInShouldReturnBadRequestWhenUserDoesNotExist()
        {
            // Arrange
            var model = new SignInModel
            {
                Email = "test@test.com"
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync((User)null);

            var controller = CreateInstance();

            // Act
            var result = await controller.SignInAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SignInShouldReturnBadRequestWhenPasswordIsIncorrect()
        {
            // Arrange
            var model = new SignInModel
            {
                Email = "test@test.com",
                Password = "sample2"
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync(new User { PasswordHash = "sample".GetHash() });

            var controller = CreateInstance();

            // Act
            var result = await controller.SignInAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SignInShouldReturnBadRequestWhenEmailDoesNotVerified()
        {
            // Arrange
            var password = "sample";
            var user = new User
            {
                PasswordHash = password.GetHash(),
                IsEmailVerified = false
            };
            var model = new SignInModel
            {
                Email = "test@test.com",
                Password = password
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            var controller = CreateInstance();

            // Act
            var result = await controller.SignInAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task SignInShouldReturnOkObjectResult()
        {
            // Arrange
            var userId = "user id";
            var password = "sample";
            var user = new User
            {
                Id = userId,
                PasswordHash = password.GetHash(),
                IsEmailVerified = true
            };
            var model = new SignInModel
            {
                Email = "test@test.com",
                Password = password
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            var controller = CreateInstance();

            // Act
            var result = await controller.SignInAsync(model);

            // Assert
            _userService.Verify(service => service.UpdateLastRequestAsync(userId), Times.Once);
            _authService.Verify(service => service.SetTokensAsync(userId), Times.Once);
            Assert.IsType<OkObjectResult>(result);
        }

        [Fact]
        public async Task ForgotPasswordShouldReturnBadRequestWhenUserDoesNotExist()
        {
            // Arrange
            var model = new ForgotPasswordModel
            {
                Email = "test@test.com"
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync((User)null);

            var controller = CreateInstance();

            // Act
            var result = await controller.ForgotPasswordAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ForgotPasswordShouldGenerateAndSendResetPasswordToken()
        {
            // Arrange
            var user = new User
            {
                Id = "user id",
                Email = "test@test.com"
            };
            var model = new ForgotPasswordModel
            {
                Email = user.Email
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            var controller = CreateInstance();

            // Act
            var result = await controller.ForgotPasswordAsync(model);

            // Assert
            _userService.Verify(service => service.UpdateResetPasswordTokenAsync(user.Id, It.IsAny<string>()), Times.Once);
            _emailService.Verify(service => service.SendForgotPassword(
                It.Is<Api.Core.Services.Infrastructure.Models.ForgotPasswordModel>(m => m.Email == user.Email))
            );

            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ResetPasswordShouldReturnBadRequestWhenTokenIsInvalid()
        {
            // Arrange
            var model = new ResetPasswordModel
            {
                Token = "test token"
            };

            _userService.Setup(service => service.FindOneAsync(It.Is<UserFilter>(filter => filter.ResetPasswordToken == model.Token)))
                .ReturnsAsync((User)null);

            var controller = CreateInstance();

            // Act
            var result = await controller.ResetPasswordAsync(model);

            // Assert
            Assert.IsType<BadRequestResult>(result);
        }

        [Fact]
        public async Task ResetPasswordShouldUpdatePassword()
        {
            // Arrange
            var model = new ResetPasswordModel
            {
                Token = "test token",
                Password = "new password"
            };
            var user = new User
            {
                Id = "user id"
            };

            _userService.Setup(service => service.FindOneAsync(It.Is<UserFilter>(filter => filter.ResetPasswordToken == model.Token)))
                .ReturnsAsync(user);

            var controller = CreateInstance();

            // Act
            var result = await controller.ResetPasswordAsync(model);

            // Assert
            _userService.Verify(service => service.UpdatePasswordAsync(user.Id, model.Password));
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task ResendVerificationShouldSendSignUpEmail()
        {
            // Arrange
            var model = new ResendVerificationModel
            {
                Email = "test@test.com"
            };
            var user = new User
            {
                SignupToken = "test token"
            };

            _userService.Setup(service => service.FindByEmailAsync(model.Email))
                .ReturnsAsync(user);

            var controller = CreateInstance();

            // Act
            var result = await controller.ResendVerificationAsync(model);

            // Assert
            _emailService.Verify(service => service.SendSignUpWelcome(
                It.Is<SignUpWelcomeModel>(m => m.Email == model.Email && m.SignUpToken == user.SignupToken))
            );
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task RefreshTokenShouldReturnUnauthorizedWhenUserIsNotFound()
        {
            // Arrange
            var refreshToken = "refresh token";
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(context => context.Request.Cookies[Constants.CookieNames.RefreshToken])
                .Returns(refreshToken);

            var controllerContext = new ControllerContext { HttpContext = contextMock.Object };

            _tokenService.Setup(service => service.FindUserIdByTokenAsync(refreshToken))
                .ReturnsAsync((string)null);

            var controller = CreateInstance();
            controller.ControllerContext = controllerContext;

            // Act
            var result = await controller.RefreshTokenAsync();

            // Assert
            Assert.IsType<UnauthorizedResult>(result);
        }

        [Fact]
        public async Task RefreshTokenShouldSetToken()
        {
            // Arrange
            var userId = "user id";
            var refreshToken = "refresh token";
            var contextMock = new Mock<HttpContext>();
            contextMock.Setup(context => context.Request.Cookies[Constants.CookieNames.RefreshToken])
                .Returns(refreshToken);

            var controllerContext = new ControllerContext { HttpContext = contextMock.Object };

            _tokenService.Setup(service => service.FindUserIdByTokenAsync(refreshToken))
                .ReturnsAsync(userId);

            var controller = CreateInstance();
            controller.ControllerContext = controllerContext;

            // Act
            var result = await controller.RefreshTokenAsync();

            // Assert
            _authService.Verify(service => service.SetTokensAsync(userId), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public async Task LogoutShouldUnsetTokenAndReturnOk()
        {
            // Arrange
            var contextMock = new Mock<HttpContext>();
            var currentUserId = "user id";

            contextMock.Setup(context => context.User.Identity.Name).Returns(currentUserId);
            var controllerContext = new ControllerContext { HttpContext = contextMock.Object };

            var controller = CreateInstance();
            controller.ControllerContext = controllerContext;

            // Act
            var result = await controller.LogoutAsync();

            // Assert
            _authService.Verify(service => service.UnsetTokensAsync(currentUserId), Times.Once);
            Assert.IsType<OkResult>(result);
        }

        [Fact]
        public void GetOAuthUrlShouldReturnRedirect()
        {
            // Arrange
            var url = "test.test";

            _googleService.Setup(service => service.GetOAuthUrl())
                .Returns(url);

            var controller = CreateInstance();

            // Act
            var result = controller.GetOAuthUrl();

            // Assert
            Assert.True(result is RedirectResult redirectResult && redirectResult.Url == url);
        }

        [Fact]
        public async Task SignInGoogleWithCodeShouldReturnNotFoundWhenPayloadIsNull()
        {
            // Arrange
            var model = new SignInGoogleModel {Code = "test code"};
            _googleService.Setup(service => service.ExchangeCodeForTokenAsync(model.Code))
                .ReturnsAsync((GooglePayloadModel)null);

            var controller = CreateInstance();

            // Act
            var result = await controller.SignInGoogleWithCodeAsync(model);

            // Assert
            Assert.IsType<NotFoundResult>(result);
        }

        [Fact]
        public async Task SignInGoogleWithCodeShouldCreateUserWhenItDoesNotExist()
        {
            // Arrange
            var model = new SignInGoogleModel { Code = "test code" };
            var payload = new GooglePayloadModel
            {
                Email = "test@test.com",
                GivenName = "Test",
                FamilyName = "Tester"
            };

            _userService.Setup(service => service.FindByEmailAsync(payload.Email))
                .ReturnsAsync((User)null);

            _userService.Setup(service => service.CreateUserAccountAsync(It.IsAny<CreateUserGoogleModel>()))
                .ReturnsAsync(new User());

            _googleService.Setup(service => service.ExchangeCodeForTokenAsync(model.Code))
                .ReturnsAsync(payload);

            var controller = CreateInstance();

            // Act
            await controller.SignInGoogleWithCodeAsync(model);

            // Assert
            _userService.Verify(service => service.CreateUserAccountAsync(
                It.Is<CreateUserGoogleModel>(m => m.Email == payload.Email &&  m.FirstName == payload.GivenName && m.LastName == payload.FamilyName)),
                Times.Once
            );
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async Task SignInGoogleWithCodeShouldShouldEnableGoogleAuthWhenItDoesNotEnabled(bool isEnabled)
        {
            // Arrange
            var model = new SignInGoogleModel { Code = "test code" };
            var payload = new GooglePayloadModel
            {
                Email = "test@test.com",
                GivenName = "Test",
                FamilyName = "Tester"
            };
            var user = new User
            {
                Id = "test id",
                Email = payload.Email,
                OAuth = new User.OAuthSettings
                {
                    Google = isEnabled
                }
            };

            _userService.Setup(service => service.FindByEmailAsync(payload.Email))
                .ReturnsAsync(user);

            _userService.Setup(service => service.CreateUserAccountAsync(It.IsAny<CreateUserGoogleModel>()))
                .ReturnsAsync(new User());

            _googleService.Setup(service => service.ExchangeCodeForTokenAsync(model.Code))
                .ReturnsAsync(payload);

            var controller = CreateInstance();

            // Act
            await controller.SignInGoogleWithCodeAsync(model);

            // Assert
            _userService.Verify(service => service.CreateUserAccountAsync(It.IsAny<CreateUserGoogleModel>()), Times.Never);
            _userService.Verify(service => service.EnableGoogleAuthAsync(user.Id), isEnabled ? Times.Never() : Times.Once());
        }

        [Fact]
        public async Task SignInGoogleWithCodeShouldShouldSetTokenAndReturnRedirect()
        {
            // Arrange
            var model = new SignInGoogleModel { Code = "test code" };
            var payload = new GooglePayloadModel
            {
                Email = "test@test.com",
                GivenName = "Test",
                FamilyName = "Tester"
            };
            var user = new User
            {
                Id = "test id",
                Email = payload.Email,
                OAuth = new User.OAuthSettings
                {
                    Google = false
                }
            };

            _userService.Setup(service => service.FindByEmailAsync(payload.Email))
                .ReturnsAsync(user);

            _googleService.Setup(service => service.ExchangeCodeForTokenAsync(model.Code))
                .ReturnsAsync(payload);


            var controller = CreateInstance();

            // Act
            var result = await controller.SignInGoogleWithCodeAsync(model);

            // Assert
            _userService.Verify(service => service.UpdateLastRequestAsync(user.Id), Times.Once);
            _authService.Verify(service => service.SetTokensAsync(user.Id), Times.Once);
            Assert.True(result is RedirectResult redirectResult && redirectResult.Url == _appSettings.WebUrl);
        }

        private AccountController CreateInstance()
        {
            _appSettingsOptions.Setup(options => options.Value)
                .Returns(_appSettings);

            return new AccountController(
                _emailService.Object, 
                _userService.Object,
                _tokenService.Object,
                _authService.Object,
                _environment.Object,
                _appSettingsOptions.Object,
                _googleService.Object,
                _mapper.Object);
        }
    }
}
