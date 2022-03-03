using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Domain;
using Api.Models;
using Api.Models.User;
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
        private readonly ShipDbContext _dbContext;

        public UsersController(
            IUserService userService,
            IMapper mapper,
            ShipDbContext dbContext)
        {
            _userService = userService;
            _mapper = mapper;
            _dbContext = dbContext;
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

            var page = await _dbContext.Users.FindPageAsync(filter, sort, model.Page, model.PerPage, map);

            return Ok(page);
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAsync()
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter
            {
                Id = CurrentUserId!.Value,
                AsNoTracking = true
            });
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

            var user = await _dbContext.Users.FindAsync(userId);
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