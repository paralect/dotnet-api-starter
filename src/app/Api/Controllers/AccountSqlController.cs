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
using Common.DALSql.Entities;
using Common.DALSql.Repositories;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ForgotPasswordModel = Api.Models.Account.ForgotPasswordModel;

namespace Api.Controllers
{
    public class AccountSqlController : BaseController
    {
        private readonly IUnitOfWork _unitOfWork;

        private readonly IEmailService _emailService;

        private readonly IUserSqlService _userSqlService;

        private readonly IAuthSqlService _authSqlService;
        private readonly IWebHostEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly IGoogleService _googleService;
        private readonly IMapper _mapper;

        public AccountSqlController(
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IUserSqlService userSqlService,
            IAuthSqlService authSqlService,
            IWebHostEnvironment environment,
            IOptions<AppSettings> appSettings,
            IGoogleService googleService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;

            _emailService = emailService;
            _userSqlService = userSqlService;
            _authSqlService = authSqlService;

            _environment = environment;
            _googleService = googleService;
            _appSettings = appSettings.Value;
            _mapper = mapper;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpModel model)
        {
            var user = await _unitOfWork.Users.FindByEmail(model.Email);
            if (user != null)
            {
                return BadRequest(nameof(model.Email), "User with this email is already registered.");
            }

            var newUser = await _userSqlService.CreateUserAccountAsync(new CreateUserModel
            {
                Email = model.Email,
                FirstName = model.FirstName,
                LastName = model.LastName,
                Password = model.Password
            });

            if (_environment.IsDevelopment())
            {
                return Ok(new {_signupToken = newUser.SignupToken});
            }

            return Ok();
        }

        [HttpGet("verifyEmail/{token}")]
        public async Task<IActionResult> VerifyEmailAsync(string token)
        {
            if (token == null)
            {
                return BadRequest("Token", "Token is required.");
            }

            var user = await _unitOfWork.Users.FindOneByQueryAsNoTracking(
                new DbQuery<User>().AddFilter(u => u.SignupToken == token)
            );
            if (user == null)
            {
                return BadRequest("Token", "Token is invalid.");
            }

            await _userSqlService.VerifyEmail(user.Id);

            return Redirect(_appSettings.WebUrl);
        }

        [HttpPost("signin")]
        public async Task<IActionResult> SignInAsync([FromBody] SignInModel model)
        {
            var user = await _unitOfWork.Users.FindByEmail(model.Email);
            if (user == null || !model.Password.IsHashEqual(user.PasswordHash))
            {
                return BadRequest("Credentials", "Incorrect email or password.");
            }

            if (user.IsEmailVerified == false)
            {
                return BadRequest(nameof(model.Email), "Please verify your email to sign in.");
            }

            await _userSqlService.SignIn(user.Id);

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordModel model)
        {
            var user = await _unitOfWork.Users.FindByEmail(model.Email);
            if (user == null)
            {
                return BadRequest(nameof(model.Email),
                    $"Couldn't find account associated with ${model.Email}. Please try again.");
            }

            var resetPasswordToken = await _userSqlService.SetResetPasswordToken(user.Id);

            _emailService.SendForgotPassword(new Core.Services.Infrastructure.Models.ForgotPasswordModel
            {
                Email = user.Email,
                ResetPasswordUrl = $"{_appSettings.LandingUrl}/reset-password?token={resetPasswordToken}",
                FirstName = user.FirstName
            });

            return Ok();
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody] ResetPasswordModel model)
        {
            var user = await _unitOfWork.Users.FindOneByQueryAsNoTracking(
                new DbQuery<User>().AddFilter(u => u.ResetPasswordToken == model.Token)
            );
            if (user == null)
            {
                return BadRequest(nameof(model.Token), "Password reset link has expired or invalid.");
            }

            await _userSqlService.UpdatePasswordAsync(user.Id, model.Password);

            return Ok();
        }

        [HttpPost("resend")]
        public async Task<IActionResult> ResendVerificationAsync([FromBody] ResendVerificationModel model)
        {
            var user = await _unitOfWork.Users.FindByEmail(model.Email);
            if (user != null)
            {
                _emailService.SendSignUpWelcome(new SignUpWelcomeModel
                {
                    Email = model.Email, SignUpToken = user.SignupToken
                });
            }

            return Ok();
        }

        [Authorize]
        [HttpPost("refresh-token")]
        public async Task<IActionResult> RefreshTokenAsync()
        {
            var refreshToken = Request.Cookies[Constants.CookieNames.RefreshToken];

            var token = await _unitOfWork.Tokens.FindOneByQueryAsNoTracking(
                new DbQuery<Token>().AddFilter(t => t.Value == refreshToken)
            );
            if (token == null || token.IsExpired())
            {
                return Unauthorized();
            }

            var tokens = _userSqlService.GenerateTokens(token.UserId);
            _unitOfWork.Tokens.AddRange(tokens);
            await _unitOfWork.Complete();

            _authSqlService.SetTokens(tokens);

            return Ok();
        }

        [HttpPost("logout")]
        public async Task<IActionResult> LogoutAsync()
        {
            if (CurrentUserId.HasValue())
            {
                await _authSqlService.UnsetTokensAsync(long.Parse(CurrentUserId));
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

            await _userSqlService.SignInGoogleWithCodeAsync(payload);

            return Redirect(_appSettings.WebUrl);
        }
    }
}