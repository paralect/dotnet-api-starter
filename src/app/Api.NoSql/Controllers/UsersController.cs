using System.Linq;
using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Document;
using Api.Models;
using Api.Models.User;
using Api.Security;
using AutoMapper;
using Common.DAL;
using Common.DAL.Interfaces;
using Common.DAL.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(IUserRepository userRepository, IUserService userService, IMapper mapper)
        {
            _userRepository = userRepository;
            _userService = userService;
            _mapper = mapper;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync([FromQuery] PageFilterModel model)
        {
            // This mapping exists only to handle sort values mismatch (-1 and 1 instead of 0 and 1).
            // For a new project it'd be better to update the client to send the same values.
            var sort = model.Sort != null
                ? model.Sort
                    .Select(x => (x.Key, x.Value == 1 ? SortDirection.Ascending : SortDirection.Descending))
                    .ToList()
                : null;

            var page = await _userRepository.FindPageAsync(
                new UserFilter { SearchValue = model.SearchValue },
                sort,
                model.Page,
                model.PerPage
            );
            var pageModel = _mapper.Map<PageModel<UserViewModel>>(page);

            return Ok(pageModel);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAsync()
        {
            var user = await _userService.FindByIdAsync(CurrentUserId);
            var viewModel = _mapper.Map<UserViewModel>(user);

            return Ok(viewModel);
        }

        [HttpPut("current")]
        public async Task<IActionResult> UpdateCurrentAsync([FromBody] UpdateCurrentModel model)
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
