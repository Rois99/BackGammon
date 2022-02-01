using Backgammon.Services.Chat.Domain.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Domain.Interfaces
{
    public interface IMessageRepo
    {
        Task<Message> CreateMessage(Guid senderId, Guid RecipientId, string MessageBody, DateTime SentAt);
        Task<IEnumerable<Message>> GetMessages(Guid senderId, Guid recipientId);
        Task SetMessageRecived(Guid messageId, DateTime recivedAt);
        Task<Message> GetMessage(Guid messageId);
    }
}
