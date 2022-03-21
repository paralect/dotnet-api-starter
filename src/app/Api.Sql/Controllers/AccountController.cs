using System.Threading.Tasks;
using AutoMapper;
using Common;
using Common.Settings;
using Common.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using ForgotPasswordModel = Api.Sql.Models.Account.ForgotPasswordModel;
using Api.Sql.Models.Account;
using Api.Sql.Services.Interfaces;
using Api.Sql.Models.User;
using Common.ServicesSql.Domain.Interfaces;
using Common.ServicesSql.Infrastructure.Interfaces;
using Common.ServicesSql.Domain.Models;
using System.Collections.Generic;
using System.Linq.Expressions;
using System;
using Common.DalSql.Entities;
using Common.DalSql.Filters;
using Api.Sql.Security;
using Common.ServicesSql.Infrastructure.Email.Models;

namespace Api.Sql.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IAuthService _authService;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly ITokenService _tokenService;
        private readonly IWebHostEnvironment _environment;
        private readonly AppSettings _appSettings;
        private readonly IMapper _mapper;

        public AccountController(
            IAuthService authService,
            IEmailService emailService,
            IUserService userService,
            ITokenService tokenService,
            IWebHostEnvironment environment,
            IOptions<AppSettings> appSettings,
            IMapper mapper)
        {
            _authService = authService;
            _emailService = emailService;
            _userService = userService;
            _tokenService = tokenService;

            _environment = environment;
            _appSettings = appSettings.Value;
            _mapper = mapper;
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

            await _userService.SignInAsync(user.Id);
            await _authService.SetTokensAsync(user.Id);

            return Ok(_mapper.Map<UserViewModel>(user));
        }

        [HttpPost("sign-up")]
        public async Task<IActionResult> SignUpAsync([FromBody] SignUpModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);
            if (user != null)
            {
                return BadRequest(nameof(model.Email), "User with this email is already registered.");
            }

            var newUser = await _userService.CreateUserAccountAsync(new CreateUserModel
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

            var user = await _userService.FindBySignupTokenAsync(token);
            if (user == null)
            {
                return BadRequest("Token", "Token is invalid.");
            }

            await _userService.VerifyEmailAsync(user.Id);
            await _authService.SetTokensAsync(user.Id);

            return Redirect(_appSettings.WebUrl);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody] ForgotPasswordModel model)
        {
            var user = await _userService.FindByEmailAsync(model.Email);
            if (user == null)
            {
                return BadRequest(nameof(model.Email),
                    $"Couldn't find account associated with ${model.Email}. Please try again.");
            }

            var resetPasswordToken = await _userService.SetResetPasswordTokenAsync(user.Id);

            _emailService.SendForgotPassword(new Common.ServicesSql.Infrastructure.Email.Models.ForgotPasswordModel
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
            var user = await _userService.FindByResetPasswordTokenAsync(model.Token);
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

            var token = await _tokenService.FindOneAsync(new TokenFilter
            {
                Value = refreshToken
            });

            if (token == null || token.IsExpired())
            {
                return Unauthorized();
            }

            await _authService.SetTokensAsync(token.UserId);

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
    }
}