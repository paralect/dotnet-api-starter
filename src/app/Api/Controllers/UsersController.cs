using System.Threading.Tasks;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.App;
using Api.Models.User;
using Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;

        public UsersController(IUserRepository userRepository,
            IUserService userService)
        {
            _userRepository = userRepository;
            _userService = userService;
        }

        [HttpGet("current")]
        public IActionResult GetCurrent()
        {
            var user = _userRepository.FindById(CurrentUserId);

            return Ok(new
            {
                user.Id,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsEmailVerified
            });
        }

        [HttpPut("current")]
        public async Task<IActionResult> UpdateCurrentAsync([FromBody]UpdateCurrentModel model)
        {
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("UserId", "User not found.");
            }

            if (_userRepository.FindOne(x => x.Id != userId && x.Email == model.Email) != null)
            {
                return BadRequest(nameof(model.Email), "This email is already in use.");
            }

            await _userService.UpdateInfoAsync(userId, model.Email, model.FirstName, model.LastName);

            var user = _userRepository.FindById(userId);
            return Ok(new
            {
                userId,
                model.FirstName,
                model.LastName,
                model.Email,
                user.IsEmailVerified
            });
        }
    }
}
