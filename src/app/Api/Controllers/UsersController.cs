using System.Threading.Tasks;
using Api.Core.DbViews.User;
using Api.Core.Interfaces.DAL;
using Api.Core.Interfaces.Services.App;
using Api.Models.User;
using Microsoft.AspNetCore.Authorization;
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
            var userId = CurrentUserId;
            if (string.IsNullOrEmpty(userId))
            {
                return BadRequest("User not found.");
            }
            
            User user = _userRepository.FindById(userId);

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
                return BadRequest("User not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            if (_userRepository.FindOne(x => x.Id != userId && x.Email == model.Email) != null)
            {
                return BadRequest("This email is already in use.");
            }

            await _userService.UpdateInfo(userId, model.Email, model.FirstName, model.LastName);

            User user = _userRepository.FindById(userId);
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
