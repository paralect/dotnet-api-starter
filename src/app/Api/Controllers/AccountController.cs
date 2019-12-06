using System.Threading.Tasks;
using Api.Core.Abstract;
using Api.Core.Models.User;
using Api.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Api.Core.Utils;
using Microsoft.Extensions.Options;
using Api.Settings;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Api.Controllers
{
    public class AccountController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;
        private readonly IWebHostEnvironment _environment;
        private readonly AppSettings _appSettings;

        public AccountController(IUserRepository userRepository, 
            IEmailService emailService, 
            IUserService userService, 
            IAuthService authService,
            IWebHostEnvironment environment,
            IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _userService = userService;
            _authService = authService;

            _environment = environment;
            _appSettings = appSettings.Value;
        }

        [HttpPost("signup")]
        public async Task<IActionResult> SignUpAsync([FromBody]SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            User user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user != null)
            {
                return BadRequest(GetErrorsModel(new { Email = "User with this email is already registered." }));
            }

            user = await _userService.CreateUserAccount(model.Email, model.FirstName, model.LastName, model.Password);

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

            User user = _userRepository.FindOne(x => x.SignupToken == token);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Token = "Token is invalid." }));
            }

            await _userService.MarkEmailAsVerified(user.Id);

            string authToken = _authService.CreateAuthToken(user.Id);
            
            return Redirect($"{_appSettings.WebUrl}?token={authToken}&emailVerification=true");
        }

        [HttpPost("signin")]
        public IActionResult Signin([FromBody]SigninModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            User user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user == null 
                || model.Password.IsHashEqual(user.PasswordHash, user.PasswordSalt) == false)
            {
                return BadRequest(GetErrorsModel(new { Credentials = "Incorrect email or password." }));
            }

            if (user.IsEmailVerified == false)
            {
                return BadRequest(GetErrorsModel(new { Email = "Please verify your email to sign in." }));
            }

            string token = _authService.CreateAuthToken(user.Id);

            return Ok(new { token });
        }

        [HttpPost("forgotPassword")]
        public async Task<IActionResult> ForgotPasswordAsync([FromBody]ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            User user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Email = $"Couldn't find account associated with ${model.Email}. Please try again." }));
            }

            string resetPasswordToken = SecurityUtils.GenerateSecureToken();
            await _userService.UpdateResetPasswordToken(user.Id, resetPasswordToken);

            _emailService.SendForgotPassword(resetPasswordToken);

            return Ok();
        }

        [HttpPost("resetPassword")]
        public async Task<IActionResult> ResetPasswordAsync([FromBody]ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            User user = _userRepository.FindOne(x => x.ResetPasswordToken == model.Token);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Token = "Password reset link has expired or invalid." }));
            }

            await _userService.UpdatePassword(user.Id, model.Password);
            await _userService.UpdateResetPasswordToken(user.Id, string.Empty);

            return Ok();
        }

        [HttpPost("resendVerification")]
        public IActionResult ResendVerification([FromBody]ResendVerificationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            User user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user != null)
            {
                _emailService.SendSignupWelcome(model.Email);
            }

            return Ok();
        }

    }
}
