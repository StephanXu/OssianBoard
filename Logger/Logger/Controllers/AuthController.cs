using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Logger.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly Services.UserService _userService;

        public AuthController(Services.UserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("sign-in")]
        public IActionResult Authentication([FromBody] Models.UserSignInRequest signInRequest)
        {
            var authResult = _userService.Authenticate(signInRequest);
            if (authResult.Success)
            {
                return Ok(new Models.AuthSuccessResult
                {
                    UserName = authResult.UserName,
                    Alias = authResult.Alias,
                    Roles = authResult.Roles,
                    Token = authResult.Token
                });
            }
            return BadRequest(new Models.AuthFailedResult
            {
                Error = authResult.Error
            });
        }

        [HttpPost]
        [Route("renew")]
        private IActionResult RenewToken([FromBody] Models.RenewTokenRequest renewRequest)
        {
            //don't use this function in final version as it is limited by authentication.
            if (!HttpContext.User.HasClaim(ClaimTypes.Name, renewRequest.UserName))
            {
                return BadRequest();
            }
            var authResult = _userService.RenewToken(renewRequest.UserName);
            if (authResult.Success)
            {
                return Ok(new Models.AuthSuccessResult
                {
                    UserName = authResult.UserName,
                    Alias = authResult.Alias,
                    Roles = authResult.Roles,
                    Token = authResult.Token
                });
            }
            return BadRequest(new Models.AuthFailedResult
            {
                Error = authResult.Error
            });
        }

        [HttpPost]
        [Route("sign-out")]
        public IActionResult SignOut()
        {
            return NoContent();
        }
    }
}