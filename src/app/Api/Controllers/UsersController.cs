using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Api.Core.Abstract;
using Api.Models.User;
using Api.Core.Models.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;

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

        [HttpGet("[controller]/current")]
        public IActionResult GetCurrent()
        {
            ObjectId userId = CurrentUserId;
            if (userId == ObjectId.Empty)
            {
                return BadRequest("User not found.");
            }
            
            User user = _userRepository.FindById(userId);

            return Ok(new
            {
                user.Id,
                user.CreatedOn,
                user.FirstName,
                user.LastName,
                user.Email,
                user.IsEmailVerified
            });
        }

        [HttpPut("[controller]/current")]
        public async Task<IActionResult> UpdateCurrent([FromBody]UpdateCurrentModel model)
        {
            ObjectId userId = CurrentUserId;
            if (userId == ObjectId.Empty)
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
                user.CreatedOn,
                model.FirstName,
                model.LastName,
                model.Email,
                user.IsEmailVerified
            });
        }
    }
}
