using System.Threading.Tasks;
using Api.Core.Services.Domain.Models;
using Api.Core.Services.Infrastructure.Models;
using Api.Core.Services.Interfaces.Domain;
using Api.Core.Services.Interfaces.Infrastructure;
using Api.Models.Account;
using Api.Models.User;
using Api.Security;
using AutoMapper;
using Common;
using Common.DALSql;
using Common.DALSql.Filters;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ForgotPasswordModel = Api.Models.Account.ForgotPasswordModel;

namespace Api.Controllers
{
    public class AccountController : BaseController
    {
        private readonly ShipDbContext _dbContext;

        private readonly IEmailService _emailService;

        private readonly IUserService _userService;

        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly IGoogleService _googleService;
        private readonly IMapper _mapper;

        public AccountController(
            ShipDbContext dbContext,
            IEmailService emailService,
            IUserService userSqlService,
            IAuthService authSqlService,
            IWebHostEnvironment environment,
            IOptions<AppSettings> appSettings,
            IGoogleService googleService,
            IMapper mapper)
        {
            _dbContext = dbContext;

            _emailService = emailService;
            _userService = userSqlService;
            _authService = authSqlService;

            _environment = environment;
            _googleService = googleService;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        [HttpPost("sign-in")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInModel model)
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                Email = model.Email
            });

            if (user == null || !model.Password.IsHashEqual(user.PasswordHash))
            {
                return BadRequest("Credentials", "Incorrect email or password.");
            }

            if (user.IsEmailVerified == false)
            {
                return BadRequest(nameof(model.Email), "Please verify your email to sign in.");
            }

            await _userService.SignInAsync(user.Id);

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpModel model)
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                Email = model.Email
            });
            if (user != null)
            {
                return BadRequest(nameof(model.Email), "User with this email is already registered.");
            }

            var newUser = _userService.CreateUserAccount(new CreateUserModel
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password
            });

            if (_environment.IsDevelopment())
            {
                return Ok(new { signupToken = newUser.SignupToken });
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

            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                SignupToken = token
            });
            if (user == null)
            {
                return BadRequest("Token", "Token is invalid.");
            }

            await _userService.VerifyEmailAsync(user.Id);

            return Redirect(_appSettings.WebUrl);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordModel model)
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                Email = model.Email
            });
            if (user == null)
            {
                return BadRequest(nameof(model.Email),
                    $"Couldn't find account associated with ${model.Email}. Please try again.");
            }

            var resetPasswordToken = await _userService.SetResetPasswordTokenAsync(user.Id);

            _emailService.SendForgotPassword(new Core.Services.Infrastructure.Models.ForgotPasswordModel
            {
                Email = user.Email,
                ResetPasswordUrl = $"{_appSettings.LandingUrl}/reset-password?token={resetPasswordToken}",
                FirstName = user.FirstName
            });

            return Ok();
        }

        [HttpPut("reset-password")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordModel model)
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                ResetPasswordToken = model.Token
            });
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
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                Email = model.Email,
                AsNoTracking = true
            });
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

            var token = await _dbContext.Tokens.FindOneByFilterAsync(new TokenFilter
            {
                Value = refreshToken,
                AsNoTracking = true
            });
            if (token == null || token.IsExpired())
            {
                return Unauthorized();
            }

            _authService.SetTokens(token.UserId);

            return Ok();
        }

        [HttpPost("sign-out")]
        public async Task<IActionResult> SignOutAsync()
        {
            if (CurrentUserId != null)
            {
                await _authService.UnsetTokensAsync(CurrentUserId.Value);
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

            await _userService.SignInGoogleWithCodeAsync(payload);

            return Redirect(_appSettings.WebUrl);
        }
    }
}