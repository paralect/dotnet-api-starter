using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_api_starter.Infrastructure.Abstract;
using dotnet_api_starter.Resources.User;
using dotnet_api_starter.Models.Account;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using dotnet_api_starter.Infrastructure.Utils;
using Microsoft.Extensions.Options;
using dotnet_api_starter.Infrastructure.Settings;
using System.Web;
using System.Collections.Specialized;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;

namespace dotnet_api_starter.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Produces("application/json")]
    public class AccountController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IEmailService _emailService;
        private readonly IUserService _userService;
        private readonly IAuthService _authService;

        public IHostingEnvironment _environment;
        private readonly AppSettings _appSettings;

        public AccountController(IUserRepository userRepository, 
            IEmailService emailService, 
            IUserService userService, 
            IAuthService authService,
            IHostingEnvironment environment,
            IOptions<AppSettings> appSettings)
        {
            _userRepository = userRepository;
            _emailService = emailService;
            _userService = userService;
            _authService = authService;

            _appSettings = appSettings.Value;
            _environment = environment;
        }


        [HttpPost]
        public async Task<IActionResult> Signup([FromBody]SignupModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            UserDocument user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user != null)
            {
                return BadRequest(GetErrorsModel(new { Email = "User with this email is already registered." }));
            }

            user = await CreateUserAccount(model);

            if (_environment.IsDevelopment())
            {
                return Ok(new { _signupToken = user.SignupToken });
            }
            return Ok();
        }
        
        [HttpGet("[controller]/[action]/{signupToken}")]
        public async Task<IActionResult> VerifyEmail(string signupToken)
        {
            if (signupToken == null)
            {
                return BadRequest("Token is required.");
            }

            UserDocument user = _userRepository.FindOne(x => x.SignupToken == signupToken);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Token = "Token is invalid." }));
            }

            await _userService.MarkEmailAsVerified(user._id);

            string authToken = _authService.CreateAuthToken(user._id);
            
            return Redirect($"{_appSettings.WebUrl}?token={authToken}&emailVerification=true");
        }

        [HttpPost]
        public IActionResult Signin([FromBody]SigninModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            UserDocument user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user == null 
                || model.Password.IsHashEqual(user.PasswordHash, user.PasswordSalt) == false)
            {
                return BadRequest(GetErrorsModel(new { Credentials = "Incorrect email or password." }));
            }

            if (user.IsEmailVerified == false)
            {
                return BadRequest(GetErrorsModel(new { Email = "Please verify your email to sign in." }));
            }

            string token = _authService.CreateAuthToken(user._id);

            return Ok(new { token });
        }

        [HttpPost]
        public async Task<IActionResult> ForgotPassword([FromBody]ForgotPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            UserDocument user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Email = $"Couldn't find account associated with ${model.Email}. Please try again." }));
            }

            string resetPasswordToken = SecurityUtils.GenerateSecureToken();
            await _userService.UpdateResetPasswordToken(user._id, resetPasswordToken);

            _emailService.SendForgotPassword(resetPasswordToken);

            return Ok();
        }

        [HttpPost]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            UserDocument user = _userRepository.FindOne(x => x.ResetPasswordToken == model.Token);
            if (user == null)
            {
                return BadRequest(GetErrorsModel(new { Token = "Password reset link has expired or invalid." }));
            }

            await _userService.UpdatePassword(user._id, model.Password);
            await _userService.UpdateResetPasswordToken(user._id, string.Empty);

            return Ok();
        }

        [HttpPost]
        public IActionResult ResendVerification([FromBody]ResendVerificationModel model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            UserDocument user = _userRepository.FindOne(x => x.Email == model.Email);
            if (user != null)
            {
                _emailService.SendSignupWelcome(model.Email);
            }

            return Ok();
        }

        private async Task<UserDocument> CreateUserAccount(SignupModel userData)
        {
            string salt = SecurityUtils.GenerateSalt();
            string hash = SecurityUtils.GetHash(userData.Password, salt);
            string signupToken = SecurityUtils.GenerateSecureToken();

            UserDocument newUser = new UserDocument
            {
                FirstName = userData.FirstName,
                LastName = userData.LastName,
                PasswordHash = hash,
                PasswordSalt = salt,
                Email = userData.Email,
                IsEmailVerified = false,
                SignupToken = signupToken
            };

            _emailService.SendSignupWelcome(userData);

            await _userRepository.Insert(newUser);

            return newUser;
        }

        private object GetErrorsModel(params object[] errors)
        {
            return new { errors };
        }
    }
}
