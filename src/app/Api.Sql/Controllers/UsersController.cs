using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Models;
using Api.Models.User;
using Api.Services.Domain;
using AutoMapper;
using Common.DALSql;
using Common.DALSql.Entities;
using Common.DALSql.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Security.Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public UsersController(
            IUserService userService,
            IMapper mapper)
        {
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
                    .Select(x => new SortField
                    {
                        FieldName = x.Key,
                        Direction = x.Value == 1 ? SortDirection.Ascending : SortDirection.Descending
                    })
                    .ToList()
                : null;

            var filter = new UserFilter
            {
                SearchValue = model.SearchValue,
                AsNoTracking = true
            };

            Expression<Func<User, UserViewModel>> map = u => new UserViewModel
            {
                Id = u.Id,
                FirstName = u.FirstName,
                LastName = u.LastName,
                Email = u.Email
            };

            var page = await _userService.FindPageAsync(filter, sort, model.Page, model.PerPage, map);

            return Ok(page);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAsync()
        {
            var user = await _userService.FindByIdAsync(CurrentUserId!.Value);
            var viewModel = _mapper.Map<UserViewModel>(user);

            return Ok(viewModel);
        }

        [HttpPut("current")]
        public async Task<IActionResult> UpdateCurrentAsync([FromBody] UpdateCurrentModel model)
        {
            if (CurrentUserId == null)
            {
                return BadRequest("UserId", "User not found.");
            }

            var userId = CurrentUserId.Value;

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