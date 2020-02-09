using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver.Core.Authentication;
using Logger.Models;

namespace Logger.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly Services.UserService _userService;

        public UserController(Services.UserService userService)
        {
            _userService = userService;
        }

        [HttpGet]
        [Authorize]
        public IActionResult GetProfile()
        {
            var profile = _userService.GetProfile(HttpContext.User.Identity.Name);
            if (null == profile)
            {
                return BadRequest("Not valid");
            }

            return Ok(profile);
        }

        [HttpGet]
        [Route("list")]
        [Authorize(Roles = "Admin")]
        public ActionResult<Models.UserList> GetUsers() =>
            Ok(new Models.UserList {Profiles = _userService.GetUsers()});

        [HttpPut]
        [Route("{userName}")]
        [Authorize(Roles = "Admin")]
        public ActionResult<UserProfile> UpdateUsers(string userName, [FromBody] Models.UserUpdateRequest updateRequest)
        {
            return Ok(_userService.Update(userName, new User
            {
                UserName = updateRequest.UserName,
                Password = updateRequest.Password,
                Alias = updateRequest.Alias,
                Roles = updateRequest.Roles
            }));
        }

        [HttpPatch]
        [Route("change-password")]
        [Authorize]
        public ActionResult<UserProfile> ChangePassword([FromBody] Models.ChangePasswordRequest changePasswordRequest)
        {
            var (status, message, user) = _userService.VerifyPassword(HttpContext.User.Identity.Name,
                changePasswordRequest.OldPassword);
            if (!status)
                return BadRequest(message);
            user.Password = changePasswordRequest.NewPassword;
            return _userService.Update(user.UserName, user);
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> RegisterUser([FromBody] Models.UserRegisterRequest registerRequest)
        {
            if (await _userService.Create(new Models.User
            {
                UserName = registerRequest.UserName,
                Password = registerRequest.Password,
                Alias = registerRequest.Alias,
                Roles = new List<string> {"Member"}
            }))
            {
                return Ok();
            }
            else
            {
                return BadRequest();
            }
        }
    }
}