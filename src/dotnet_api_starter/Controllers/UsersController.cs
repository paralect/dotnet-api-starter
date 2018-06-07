using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using dotnet_api_starter.Infrastructure.Abstract;
using dotnet_api_starter.Models.User;
using dotnet_api_starter.Resources.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

namespace dotnet_api_starter.Controllers
{
    [EnableCors("AllowSpecificOrigin")]
    [Produces("application/json")]
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

        [Authorize]
        [HttpGet("[controller]/current")]
        public IActionResult GetCurrent()
        {
            ObjectId userId = GetAuthorizedUserId();
            if (userId == ObjectId.Empty)
            {
                return BadRequest("User not found.");
            }

            UserDocument user = _userRepository.FindById(userId);

            return Ok(new
            {
                user._id,
                user.CreatedOn,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsEmailVerified
            });
        }

        [Authorize]
        [HttpPut("[controller]/current")]
        public async Task<IActionResult> UpdateCurrent([FromBody]UpdateCurrentModel model)
        {
            ObjectId userId = GetAuthorizedUserId();
            if (userId == ObjectId.Empty)
            {
                return BadRequest("User not found.");
            }

            if (!ModelState.IsValid)
            {
                return BadRequest(GetErrorsFromModelState(ModelState));
            }

            if (_userRepository.FindOne(x => x._id != userId && x.Email == model.Email) != null)
            {
                return BadRequest("This email is already in use.");
            }

            await _userService.UpdateInfo(userId, model.Email, model.FirstName, model.LastName);

            UserDocument user = _userRepository.FindById(userId);
            return Ok(new
            {
                userId,
                user.CreatedOn,
                model.FirstName,
                model.LastName,
                model.Email,
                user.IsEmailVerified
            });
        }

        private ObjectId GetAuthorizedUserId()
        {
            ObjectId userId = ObjectId.Empty;
            ObjectId.TryParse(User.Claims.FirstOrDefault(x => x.Type == "_id")?.Value, out userId);
            return userId;
        }
    }
}
