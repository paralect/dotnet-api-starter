﻿using System.Collections.Generic;
using System.Threading.Tasks;
using Api.Core.Services.Interfaces.Domain;
using Api.Models.User;
using Api.Security;
using AutoMapper;
using Common.DAL.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Api.Controllers
{
    //[Authorize]
    public class UsersController : BaseController
    {
        private readonly IUserService _userService;
        private readonly IUserRepository _userRepository;
        private readonly IMapper _mapper;

        public UsersController(IUserService userService, IMapper mapper, IUserRepository userRepository)
        {
            _userService = userService;
            _mapper = mapper;
            _userRepository = userRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetAsync()
        {
            var users = await _userRepository.GetQueryable().ToCursorAsync();
            var viewModel = _mapper.Map<IList<UserViewModel>>(users);

            return Ok(viewModel);
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
