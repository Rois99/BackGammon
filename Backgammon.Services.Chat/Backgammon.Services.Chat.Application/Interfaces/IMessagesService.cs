using Backgammon.Services.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Application.Interfaces
{
    public interface IMessagesService
    {
        Task<Message> AddMessage(Guid senderId, Guid RecipientId, string MessageBody, DateTime SentAt);
        Task<Message> GetMessage(Guid messageId, Guid recipientId);
        Task SetAsRecived(Guid messageId, Guid recipientId, DateTime recivedAt);
        Task<IEnumerable<Message>> GetConversation(Guid chatter1Id, Guid chatter2Id);
    }
}
