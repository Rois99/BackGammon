using Backgammon.Services.Chat.Application.Interfaces;
using Backgammon.Services.Chat.Domain.Interfaces;
using Backgammon.Services.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Application.Services
{
    public class MessagesService : IMessagesService
    {
        private readonly IMessageRepo messageRepo;

        public MessagesService(IMessageRepo messageRepo)
        {
            this.messageRepo = messageRepo;
        }

        public async Task<Message> AddMessage(Guid senderId, Guid RecipientId, string MessageBody, DateTime SentAt)
        {
            var message = await messageRepo.CreateMessage(senderId, RecipientId, MessageBody, SentAt);
            return message;
        }

        public async Task<IEnumerable<Message>> GetConversation(Guid chatter1Id, Guid chatter2Id)
        {
            var chatter1Messages = await messageRepo.GetMessages(chatter1Id, chatter2Id);
            var chatter2Messages = await messageRepo.GetMessages(chatter2Id, chatter1Id);

            var orderedConversation = chatter1Messages.Concat(chatter2Messages).OrderBy(m => m.SentAt).ToList();
            return orderedConversation;
        }

        public async Task<Message> GetMessage(Guid messageId,Guid recipientId)
        {
            var message = await messageRepo.GetMessage(messageId);
            if (message.RecipientId != recipientId)
                return null;

            return message;
        }

        public async Task SetAsRecived(Guid messageId, Guid recipientId, DateTime recivedAt)
        {
            var message = await messageRepo.GetMessage(messageId);
            if (message.RecipientId != recipientId)
                return;

            await messageRepo.SetMessageRecived(messageId, recivedAt);
        }
    }
}
