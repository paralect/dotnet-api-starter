using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Domain;
using Api.Models.User;
using AutoMapper;
using Common.DALSql;
using Common.DALSql.Filters;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Security.Authorize]
    public class UsersSqlController : BaseSqlController
    {
        private readonly IUserSqlService _userSqlService;
        private readonly IMapper _mapper;
        private readonly ShipDbContext _dbContext;

        public UsersSqlController(
            IUserSqlService userSqlService,
            IMapper mapper,
            ShipDbContext dbContext)
        {
            _userSqlService = userSqlService;
            _mapper = mapper;
            _dbContext = dbContext;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAsync()
        {
            var user = await _dbContext.Users.FindOneByFilterAsync(new UserFilter {
                Id = CurrentUserId!.Value,
                AsNoTracking = true
            });
            var viewModel = _mapper.Map<UserViewModel>(user);

            return Ok(viewModel);
        }

        [HttpPut("current")]
        public async Task<IActionResult> UpdateCurrentAsync([FromBody]UpdateCurrentModel model)
        {
            if (CurrentUserId == null)
            {
                return BadRequest("UserId", "User not found.");
            }

            var userId = CurrentUserId.Value;
            
            var isEmailInUse = await _userSqlService.IsEmailInUseAsync(userId, model.Email);
            if (isEmailInUse)
            {
                return BadRequest(nameof(model.Email), "This email is already in use.");
            }
        
            await _userSqlService.UpdateInfoAsync(userId, model.Email, model.FirstName, model.LastName);

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
