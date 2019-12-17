using System.Threading.Tasks;
using Api.Core;
using Api.Core.Interfaces.Services.App;
using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Services.App.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Settings;
using Api.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Api.Core.Utils;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Api.Security;
using ForgotPasswordModel = Api.Models.Account.ForgotPasswordModel;

namespace Api.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly IGoogleService _googleService;

        public AccountController(
            IEmailService emailService,
            IUserService userService,
            ITokenService tokenService,
            IAuthService authService,
            IWebHostEnvironment environment,
            IOptions<AppSettings> appSettings,
            IGoogleService googleService)
        {
            _emailService = emailService;
            _userService = userService;
            _tokenService = tokenService;
            _authService = authService;

            _environment = environment;
            _googleService = googleService;
            _appSettings = appSettings.Value;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync([FromBody]SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            var user = _userService.FindByEmail(model.Email);
            if (user != null)
            {
                return BadRequest(GetErrorsModel(new { Email = "User with this email is already registered." }));
            }

            user = await _userService.CreateUserAccount(new CreateUserModel
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password
            });

            if (_environment.IsDevelopment())
            {
                return Ok(new { _signupToken = user.SignupToken });
            }

            return Ok();
        }

        [HttpGet("verifyEmail/{token}")]
        public async Task<IActionResult> VerifyEmail(string token)
        {
            if (token == null)
            {
                return BadRequest("Token is required.");
            }

            var user = _userService.FindBySignupToken(token);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Token = "Token is invalid." }));
            }

            var userId = user.Id;

            await Task.WhenAll(
                _userService.MarkEmailAsVerified(userId),
                _userService.UpdateLastRequest(userId),
                _authService.SetTokens(userId)
            );

            return Redirect(_appSettings.WebUrl);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> Signin([FromBody]SigninModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            var user = _userService.FindByEmail(model.Email);
            if (user == null || !model.Password.IsHashEqual(user.PasswordHash))
            {
                return BadRequest(GetErrorsModel(new { Credentials = "Incorrect email or password." }));
            }

            if (user.IsEmailVerified == false)
            {
                return BadRequest(GetErrorsModel(new { Email = "Please verify your email to sign in." }));
            }

            await Task.WhenAll(
                _userService.UpdateLastRequest(user.Id),
                _authService.SetTokens(user.Id)
            );

            return new JsonResult(new { redirectUrl =_appSettings.WebUrl });
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody]ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            var user = _userService.FindByEmail(model.Email);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Email = $"Couldn't find account associated with ${model.Email}. Please try again." }));
            }

            var resetPasswordToken = user.ResetPasswordToken;
            if (resetPasswordToken.HasNoValue())
            {
                resetPasswordToken = SecurityUtils.GenerateSecureToken();
                await _userService.UpdateResetPasswordToken(user.Id, resetPasswordToken);
            }

            _emailService.SendForgotPassword(new Core.Services.Infrastructure.Models.ForgotPasswordModel
            {
                Email = user.Email,
                ResetPasswordUrl = $"{_appSettings.LandingUrl}/reset-password?token={resetPasswordToken}",
                FirstName = user.FirstName
            });

            return Ok();
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody]ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            var user = _userService.FindByResetPasswordToken(model.Token);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Token = "Password reset link has expired or invalid." }));
            }

            await _userService.UpdatePassword(user.Id, model.Password);

            return Ok();
        }

        [HttpPost("resend")]
        public IActionResult ResendVerification([FromBody]ResendVerificationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            var user = _userService.FindByEmail(model.Email);
            if (user != null)
            {
                _emailService.SendSignupWelcome(new SignupWelcomeModel
                {
                    Email = model.Email,
                    SignupToken = user.SignupToken
                });
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies[Constants.CookieNames.RefreshToken];

            var userId = _tokenService.FindUserIdByToken(refreshToken);
            if (userId.HasNoValue())
            {
                return Unauthorized();
            }

            await _authService.SetTokens(userId);

            return Ok();
        }

        [Authorize]
        [HttpGet("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            await _authService.UnsetTokens(CurrentUserId);

            return Ok();
        }

        [HttpGet("signin/google/auth")]
        public IActionResult GetOAuthUrl()
        {
            var url = _googleService.GetOAuthUrl();
            return Redirect(url);
        }

        [HttpGet("signin/google")]
        public async Task<IActionResult> SigninGoogleWithCodeAsync([FromQuery]SigninGoogleModel model)
        {
            var payload = await _googleService.ExchangeCodeForToken(model.Code);
            if (payload == null)
            {
                return NotFound();
            }

            var user = _userService.FindByEmail(payload.Email);
            if (user != null && !user.OAuth.Google)
            {
                await _userService.EnableGoogleAuth(user.Id);
            }
            else
            {
                user = await _userService.CreateUserAccount(new CreateUserGoogleModel
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName
                });
            }

            await Task.WhenAll(
                _userService.UpdateLastRequest(user.Id),
                _authService.SetTokens(user.Id)
            );

            return Redirect(_appSettings.WebUrl);
        }
    }
}
