using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Api.Contracts.Requests;
using Backgammon.Services.Identity.Api.Contracts.Responses;
using Backgammon.Services.Identity.Api.Extensions;
using Backgammon.Services.Identity.Application.Interfaces;
using Backgammon.Services.Identity.Application.Results;
using Backgammon.Services.Identity.Contracts.Requests;
using Backgammon.Services.Identity.Contracts.Responses;
using Backgammon.Services.Identity.Extensions;

namespace TalkBack.Services.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService userService;
        private readonly IAuthService auhtService;

        public UserController(IUserService userService,IAuthService auhtService)
        {
            this.userService = userService;
            this.auhtService = auhtService;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var authenticatedId = User.GetId();

            if (string.IsNullOrEmpty(authenticatedId))
                return BadRequest(new FailedResponse());

            if(!Guid.TryParse(authenticatedId,out Guid userId))
                return BadRequest(new FailedResponse());

            var nameResult = await userService.GetUsername(userId);

            if (!nameResult.IsSuccess)
                return BadRequest(new FailedResponse()
                {
                    Errors = ((FailedResult)nameResult).Errors
                });

            var result = (NameResult)nameResult;

            return Ok(new UsernameResponse() { Username = result.Userame});
        }

        [HttpPost("Register")]
        public async Task<IActionResult> Register(RegisterRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(new FailedResponse()
                {
                    Errors = ModelState.GetAllErrors()
                });

            var result = await userService.RegisterNewUserAsync(request.Username, request.Password,request.ConfirmPassword);

            if (!result.IsSuccess)
                return BadRequest(new FailedResponse()
                {
                    Errors = ((FailedResult)result).Errors
                }
                );

            var authResult = await auhtService.LoginAsync(request.Username, request.Password);

            if (!authResult.IsSuccess)
                return BadRequest(new FailedResponse()
                {
                    Errors = ((FailedResult)result).Errors
                    .Append("User created but didn't auto login.")
                }
                );

            var auth = (AuthResult)authResult;

            return Ok(new AuthResponse()
            {
                AccessToken = auth.AccessToken,
                RefreshToken = auth.RefreshToken         
            });
        }

        [Authorize]
        [HttpPatch]
        public async Task<IActionResult> ChangeName(RenameRequest request)
        {         
            if (string.IsNullOrEmpty(request.NewUsername))
                return BadRequest(new FailedResponse()
                {
                    Errors = new[]
                    {
                        "The new name must be specified."
                    }
                });

            var authenticatedId = User.GetId();

            if (string.IsNullOrEmpty(authenticatedId))
                return BadRequest(new FailedResponse());

            if (!Guid.TryParse(authenticatedId, out Guid userId))
                return BadRequest(new FailedResponse());

            var result = await userService.ChangeUserName(userId, request.NewUsername);
            if(!result.IsSuccess)
                return BadRequest(new FailedResponse()
                {
                    Errors = ((FailedResult)result).Errors
                });

            return Ok();
        }
    }
}
