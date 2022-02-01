using Backgammon.Services.Chat.Api.Contracts.Requests;
using Backgammon.Services.Chat.Api.Contracts.Responses;
using Backgammon.Services.Chat.Application.Interfaces;
using Backgammon.Services.Chat.Application.Services;
using Backgammon.Services.Chat.Domain.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Api.Hubs
{
    [Authorize]
    public class ChatHub : Hub
    {
        private readonly IChatService chatService;
        private readonly IMessagesService messagesService;

        public ChatHub(IChatService chatService,IMessagesService messagesService)
        {
            this.chatService = chatService;
            this.messagesService = messagesService;
        }

        public override async Task OnConnectedAsync()
        {
            await base.OnConnectedAsync();
            if (!Guid.TryParse(Context.UserIdentifier, out var chaterId))
                return;

            var userName = Context.User.FindFirst("name").Value;

            // Add user to connected list, 
            var chatter = await chatService.GetOrAddChatterAsync(chaterId,userName);

            var isFirstConnect = await chatService.ConnectChatterAsync(chatter.Id, Context.ConnectionId);

            var chattersWithoutCaller = await chatService.GetChattersAsync(chaterId);

            if (isFirstConnect)
            {
                var chatterIds = chattersWithoutCaller.Select(c => c.Id.ToString()).ToList();

                await Clients.Users(chatterIds).SendAsync("ChatterConnected", chatter);
            }

            await Clients.Caller.SendAsync("SetChatters", chattersWithoutCaller);      
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            await base.OnDisconnectedAsync(exception);
            if (!Guid.TryParse(Context.UserIdentifier, out var chaterId))
                return;

            var userName = Context.User.FindFirst("name").Value;
            // Add user to connected list, 
            var chatter = await chatService.GetChatterAsync(chaterId);
            if (chatter == null)
                return;
            if(await chatService.DisconnectChatterAsync(chatter.Id, Context.ConnectionId))
            {
                var lastSeen = await chatService.GetLastSeen(chatter.Id);
                chatter.LastSeen = lastSeen;
                await Clients.Others.SendAsync("ChatterDisconnect",chatter);
            }
                
        }

        public async Task SendMessage(MessageRequest messageRequest)
        {
            if (!Guid.TryParse(Context.UserIdentifier, out var senderId))
                // invalid
                return;

            // Validate request:
            if (string.IsNullOrEmpty(messageRequest.MessageBody))
                // invalid
                return;

            // Validate recipient
            var recipient = await chatService.GetChatterAsync(messageRequest.RecipientId);
           if (recipient == null)
                // invalid
                return;

            if (!recipient.IsConnected)
                // invalid
                return;

            // Validate Sent date
            if (DateTime.UtcNow < messageRequest.SentAt)
                // invalid
                return;

            // Create & Add message to db
            var message = await messagesService.AddMessage(senderId, messageRequest.RecipientId,messageRequest.MessageBody,messageRequest.SentAt);

            // Send message to the recipient
            await Clients.Caller.SendAsync("SetMessage", message);
            await Clients.User(messageRequest.RecipientId.ToString()).SendAsync("SetMessage", message);
        }

        public async Task ConfirmMessage(ConfirmRequest confirm)
        {
            if (!Guid.TryParse(Context.UserIdentifier, out var recipientId))
                // invalid
                return;

            if (confirm.ReceivedAt > DateTime.UtcNow)
                // invalid
                return;

            var message = await messagesService.GetMessage(confirm.MessageId, recipientId);
            if (message == null)
                // invalid
                return;

            if (message.IsReceived)
                // invalid
                return;

            if (message.SentAt > confirm.ReceivedAt)
                // invalid
                return;

            await messagesService.SetAsRecived(confirm.MessageId, recipientId, confirm.ReceivedAt);

            var sender = await chatService.GetChatterAsync(message.SenderId);
            if(sender.IsConnected)
                // Send message to the sender
                await Clients.User(sender.Id.ToString()).SendAsync("ConfirmMessage", new ConfirmMessage 
                { 
                    MessageId = confirm.MessageId,
                    ReceivedAt =confirm.ReceivedAt 
                });
        
        }
    }
}
