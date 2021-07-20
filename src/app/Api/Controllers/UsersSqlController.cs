using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Domain;
using Api.Models.User;
using AutoMapper;
using Common.DALSql;
using Common.DALSql.Entities;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Api.Controllers
{
    [Security.Authorize]
    public class UsersSqlController : BaseSqlController
    {
        private readonly IUserSqlService _userSqlService;
        private readonly IMapper _mapper;
        private readonly DbSet<User> _users;

        public UsersSqlController(
            IUserSqlService userSqlService,
            IMapper mapper,
            IUnitOfWork unitOfWork)
        {
            _userSqlService = userSqlService;
            _mapper = mapper;
            _users = unitOfWork.Users;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAsync()
        {
            var user = await _users.FindOneAsNoTracking(CurrentUserId.Value);
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

            var user = await _users.FindAsync(userId);
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
