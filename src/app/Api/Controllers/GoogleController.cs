using System.Threading.Tasks;
using Api.Core.DbViews.User;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.App;
using Api.Core.Interfaces.Services.Infrastructure;
using Api.Core.Services.App.Models;
using Api.Core.Settings;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

namespace Api.Controllers
{
    public class GoogleController : BaseController
    {
        private readonly AppSettings _appSettings;
        private readonly IGoogleService _googleService;
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IAuthService _authService;

        public GoogleController(
            IOptions<AppSettings> appSettings,
            IGoogleService googleService,
            IUserService userService,
            IUserRepository userRepository,
            IAuthService authService
        )
        {
            _appSettings = appSettings.Value;
            _googleService = googleService;
            _userService = userService;
            _userRepository = userRepository;
            _authService = authService;
        }

        public IActionResult GetOAuthUrl()
        {
            var url = _googleService.GetOAuthUrl();
            return Redirect(url);
        }

        public async Task<IActionResult> SigninGoogleWithCode(string code)
        {
            var result = _googleService.ExchangeCodeForToken(code);
            if (result.IsValid)
            {
                return NotFound();
            }

            var user = _userService.FindByEmail(result.Payload.Email);
            if (user != null)
            {
                if (!user.OAuth.Google)
                {
                    await _userRepository.Update(user.Id,
                        u => new User {OAuth = new User.OAuthSettings {Google = true}});
                    user = _userRepository.FindById(user.Id); // TODO single request to DB
                }
            }
            else
            {
                var payload = result.Payload;
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
