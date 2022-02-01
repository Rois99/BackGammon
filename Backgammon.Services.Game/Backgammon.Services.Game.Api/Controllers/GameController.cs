using Backgammon.Services.Game.App.Interfaces;
using Backgammon.Services.Game.Domain.Interfaces;
using Backgammon.Services.Game.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Backgammon.Services.Game.Api.Controllers
{
    [ApiController]
    [Route ("api/[controller]")]
    [Authorize]
    public class GameController : ControllerBase
    {
        ICubeService _cubeService;
        IPlayerService _playerService;
        public GameController(ICubeService cubeService, IPlayerService playerService)
        {
            _cubeService = cubeService;
            _playerService = playerService;
        }

        public async Task<string> RollCubes()
        {
            int[] nums = new int[2];
            nums[0] = _cubeService.RollCube().Result;
            Thread.Sleep(1000);
            nums[1] = _cubeService.RollCube().Result;
            return nums.ToString();
        }

        [HttpGet("Stats/{playerId}")]
        public IActionResult GetPlayersStats(string playerId)
        {
            if (string.IsNullOrEmpty(playerId))
                return BadRequest("player id was null");
            var playerStats = _playerService.GetPlayerStats(playerId);
            if (playerStats == null)
                return BadRequest("player was not found");
            return Ok(_playerService.GetPlayerStats(playerId));
        }

        
        public Dictionary<string, Player> UpdateOnlineUsers() => _playerService.Players;
    }



}
