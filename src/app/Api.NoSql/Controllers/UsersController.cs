using System.Linq;
using System.Threading.Tasks;
using Api.Views.Models.View;
using Api.Views.Models.View.User;
using AutoMapper;
using Common.Caching.Interfaces;
using Common.Dal;
using Common.Dal.Documents.User;
using Common.Dal.Repositories;
using Common.Security;
using Common.Services.NoSql.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.NoSql.Controllers
{
    [Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly ICache _cache;

        public UsersController(IUserService userService,
            ICache cache,
            IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
            _cache = cache;
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

            var page = await _userService.FindPageAsync(
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
            var user = await _cache.GetAsync<User>(CurrentUserId);

            if (user == null)
            {
                user = await _userService.FindByIdAsync(CurrentUserId);
                await _cache.SetAsync(CurrentUserId, user);
            }

            var viewModel = _mapper.Map<UserViewModel>(user);

            return Ok(viewModel);
        }

        // TODO change to PUT on client side and here
        [HttpPost("current")]
        public async Task<IActionResult> UpdateCurrentAsync([FromBody] UpdateCurrentUserModel model)
        {
            await _userService.UpdatePasswordAsync(CurrentUserId, model.Password);

            var user = await _userService.FindByIdAsync(CurrentUserId);
            return Ok(new
            {
                CurrentUserId,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsEmailVerified
            });
        }
    }
}
