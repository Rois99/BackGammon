using Backgammon.Services.Chat.Api.Extensions;
using Backgammon.Services.Chat.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class HistoryController : ControllerBase
    {
        private readonly IMessagesService messagesService;
        private readonly IChatService chatService;

        public HistoryController(IMessagesService messagesService,IChatService chatService)
        {
            this.messagesService = messagesService;
            this.chatService = chatService;
        }
        [HttpGet]
        public async Task<IActionResult> Get([FromQuery] Guid chatWithId)
        {
            if (!Guid.TryParse(HttpContext.User.GetId(), out var chatterId))
                // invalid
                return BadRequest("Wat"); // TODO: proper failrequest

            // Validate request:
            if (chatWithId == Guid.Empty)
                // invalid
                return BadRequest("Wat"); // TODO: proper failrequest

            // Validate chater
            var chatter = await chatService.GetChatterAsync(chatterId);
            if (chatter == null)
                // invaild
                return BadRequest("Wat");

            // Validate chater
            var chatWith = await chatService.GetChatterAsync(chatWithId);
            if (chatWith == null)
                // invaild
                return BadRequest("Wat");

            var conversation = messagesService.GetConversation(chatterId, chatWithId);

            return Ok(conversation);
        }
    }
}
