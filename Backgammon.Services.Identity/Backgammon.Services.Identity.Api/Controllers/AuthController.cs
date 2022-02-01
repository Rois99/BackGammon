using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Backgammon.Services.Identity.Api.Contracts.Requests;
using Backgammon.Services.Identity.Application.Interfaces;
using Backgammon.Services.Identity.Application.Results;
using Backgammon.Services.Identity.Contracts.Requests;
using Backgammon.Services.Identity.Contracts.Responses;
using Backgammon.Services.Identity.Extensions;

namespace Backgammon.Services.Identity.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService service;

        public AuthController(IAuthService service)
        {
            this.service = service;
        }

        [HttpGet("Ping")]
        [Authorize]
        public IActionResult Ping()
        {
            return Ok(new { Message="Pong!" });
        }

        [HttpPost("Login")]
        public async Task<IActionResult> Login(LoginRequest requset)
        {
            if (!ModelState.IsValid)
                return BadRequest(new FailedResponse() { Errors = ModelState.GetAllErrors() });

            var result = await service.LoginAsync(requset.Username, requset.Password);

            if (!result.IsSuccess)
                return BadRequest(new FailedResponse() { Errors = ((FailedResult)result).Errors });

            var authResult = (AuthResult)result;

            return Ok(new AuthResponse()
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken
            });
        }

        [HttpPost("Logout")]
        public async Task<IActionResult> Logout(LogoutRequest requset)
        {
            if (!ModelState.IsValid)
                return BadRequest(new FailedResponse() { Errors = ModelState.GetAllErrors() });

            var result = await service.LogoutAsync(requset.RefreshToken);

            if (!result.IsSuccess)
                return BadRequest(new FailedResponse() { Errors = ((FailedResult)result).Errors });

            return Ok();
        }


        [HttpPost("Refresh")]
        public async Task<IActionResult> Refresh(RefreshRequest requset)
        {
            if (!ModelState.IsValid)
                return BadRequest(new FailedResponse() { Errors = ModelState.GetAllErrors() });

            var result = await service.RefreshAsync(requset.AccessToken,requset.RefreshToken);

            if (!result.IsSuccess)
                return BadRequest(new FailedResponse() { Errors = ((FailedResult)result).Errors });

            var authResult = (AuthResult)result;

            return Ok(new AuthResponse()
            {
                AccessToken = authResult.AccessToken,
                RefreshToken = authResult.RefreshToken
            });
        }

    }
}
