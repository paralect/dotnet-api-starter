using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Domain;
using Api.Models.User;
using Api.Security;
using AutoMapper;
using Common.DALSql.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    [Authorize]
    public class UsersSqlController : BaseSqlController
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IUserSqlService _userSqlService;
        private readonly IMapper _mapper;

        public UsersSqlController(
            IUnitOfWork unitOfWork,
            IUserSqlService userSqlService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _userSqlService = userSqlService;
            _mapper = mapper;
        }

        [HttpGet("current")]
        public async Task<IActionResult> GetCurrentAsync()
        {
            var user = await _unitOfWork.Users.FindAsNoTracking(CurrentUserId.Value);
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

            var user = await _unitOfWork.Users.FindAsNoTracking(userId); // TODO check if needed
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
