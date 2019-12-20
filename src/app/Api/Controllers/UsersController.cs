using System.Threading.Tasks;
using Api.Core.Interfaces.Services.App;
using Api.Models.User;
using Api.Security;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;

        public UsersController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAsync()
        {
            var user = await _userService.FindByIdAsync(CurrentUserId);

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

            var isEmailInUse = await _userService.IsEmailInUseAsync(userId, model.Email);
            if (isEmailInUse)
            {
                return BadRequest(nameof(model.Email), "This email is already in use.");
            }

            await _userService.UpdateInfoAsync(userId, model.Email, model.FirstName, model.LastName);

            var user = await _userService.FindByIdAsync(userId);
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
