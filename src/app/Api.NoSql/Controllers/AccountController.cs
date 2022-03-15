using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Dal.Repositories;
using Common.Services.Domain.Interfaces;
using Common.Services.Domain.Models;
using Common.Services.Infrastructure.Interfaces;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ForgotPasswordModel = Api.NoSql.Models.Account.ForgotPasswordModel;
using Api.NoSql.Services.Interfaces;
using Api.NoSql.Models.Account;
using Api.NoSql.Models.User;
using Api.NoSql.Security;
using Common.Services.Infrastructure.Email.Models;

namespace Api.NoSql.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IAuthService _authService;
        private readonly IGoogleService _googleService;

        private readonly IWebHostEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public AccountController(
            IEmailService emailService,
            IUserService userService,
            ITokenService tokenService,
            IAuthService authService,
            IGoogleService googleService,
            IWebHostEnvironment environment,
            IOptions<AppSettings> appSettings,
            IMapper mapper)
        {
            _emailService = emailService;
            _userService = userService;
            _tokenService = tokenService;
            _authService = authService;
            _googleService = googleService;

            _environment = environment;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);
            if (user != null)
            {
                return BadRequest(nameof(model.Email), "User with this email is already registered.");
            }

            user = await _userService.CreateUserAccountAsync(new CreateUserModel
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password
            });

            if (_environment.IsDevelopment())
            {
                return Ok(new { signupToken = user.SignupToken });
            }

            return Ok();
        }

        [HttpGet("verify-email")]
        public async Task<IActionResult> VerifyEmailAsync([FromQuery] string token)
        {
            if (token == null)
            {
                return BadRequest("Token", "Token is required.");
            }

            var user = await _userService.FindOneAsync(new UserFilter { SignUpToken = token });
            if (user == null)
            {
                return BadRequest("Token", "Token is invalid.");
            }

            var userId = user.Id;

            await Task.WhenAll(
                _userService.MarkEmailAsVerifiedAsync(userId),
                _userService.UpdateLastRequestAsync(userId),
                _authService.SetTokensAsync(userId, user.Role)
            );

            return Redirect(_appSettings.WebUrl);
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);
            if (user == null || !model.Password.IsHashEqual(user.PasswordHash))
            {
                return BadRequest("Credentials", "Incorrect email or password.");
            }

            if (user.IsEmailVerified == false)
            {
                return BadRequest(nameof(model.Email), "Please verify your email to sign in.");
            }

            await Task.WhenAll(
                _userService.UpdateLastRequestAsync(user.Id),
                _authService.SetTokensAsync(user.Id, user.Role)
            );

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(nameof(model.Email), $"Couldn't find account associated with ${model.Email}. Please try again.");
            }

            var resetPasswordToken = user.ResetPasswordToken;
            if (resetPasswordToken.HasNoValue())
            {
                resetPasswordToken = SecurityUtils.GenerateSecureToken();
                await _userService.UpdateResetPasswordTokenAsync(user.Id, resetPasswordToken);
            }

            _emailService.SendForgotPassword(new Common.Services.Infrastructure.Email.Models.ForgotPasswordModel
            {
                Email = user.Email,
                ResetPasswordUrl = $"{_appSettings.LandingUrl}/reset-password?token={resetPasswordToken}",
                FirstName = user.FirstName
            });

            return Ok();
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordModel model)
        {
            var user = await _userService.FindOneAsync(new UserFilter { ResetPasswordToken = model.Token });
            if (user == null)
            {
                return BadRequest(nameof(model.Token), "Password reset link has expired or invalid.");
            }

            await _userService.UpdatePasswordAsync(user.Id, model.Password);

            return Ok();
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendVerificationAsync([FromBody] ResendVerificationModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);
            if (user != null)
            {
                _emailService.SendSignUpWelcome(new SignUpWelcomeModel
                {
                    Email = model.Email,
                    SignUpToken = user.SignupToken
                });
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies[Constants.CookieNames.RefreshToken];

            var token = await _tokenService.FindByValueAsync(refreshToken);
            if (token == null || token.IsExpired())
            {
                return Unauthorized();
            }

            await _authService.SetTokensAsync(token.UserId, token.UserRole);

            return Ok();
        }

        [HttpPost("sign-out")]
        public async Task<IActionResult> SignOutAsync()
        {
            if (CurrentUserId.HasValue())
            {
                await _authService.UnsetTokensAsync(CurrentUserId);
            }

            return Ok();
        }

        [HttpGet("signin/google/auth")]
        public IActionResult GetOAuthUrl()
        {
            var url = _googleService.GetOAuthUrl();

            return Redirect(url);
        }

        [HttpGet("signin/google")]
        public async Task<IActionResult> SignInGoogleWithCodeAsync([FromQuery] SignInGoogleModel model)
        {
            var payload = await _googleService.ExchangeCodeForTokenAsync(model.Code);
            if (payload == null)
            {
                return NotFound();
            }

            var user = await _userService.FindByEmailAsync(payload.Email);
            if (user == null)
            {
                user = await _userService.CreateUserAccountAsync(new CreateUserGoogleModel
                {
                    Email = payload.Email,
                    FirstName = payload.GivenName,
                    LastName = payload.FamilyName
                });
            }
            else if (!user.OAuth.Google)
            {
                await _userService.EnableGoogleAuthAsync(user.Id);
            }

            await Task.WhenAll(
                _userService.UpdateLastRequestAsync(user.Id),
                _authService.SetTokensAsync(user.Id, user.Role)
            );

            return Redirect(_appSettings.WebUrl);
        }
    }
}
