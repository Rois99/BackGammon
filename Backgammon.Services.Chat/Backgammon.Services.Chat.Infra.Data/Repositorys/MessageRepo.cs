using Backgammon.Services.Chat.Domain.Interfaces;
using Backgammon.Services.Chat.Domain.Models;
using Backgammon.Services.Chat.Infra.Data.Dtos;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Backgammon.Services.Chat.Infra.Data.Repositorys
{
    public class MessageRepo : IMessageRepo
    {
        private readonly DataContext context;

        public MessageRepo(DataContext context)
        {
            this.context = context;
        }

        public async Task<Message> GetMessage(Guid messageId)
        {
            var dto = await context.Messages.FindAsync(messageId);
            if (dto == null)
                return null;

            return new Message
            {
                Id = dto.Id,
                ReceivedAt = dto.ReceivedAt,
                SenderId = dto.SenderId,
                IsReceived = dto.IsRecived,
                MessageBody = dto.MessageBody,
                RecipientId = dto.RecipientId,
                SentAt = dto.SentAt
            };
        }

        public async Task SetMessageRecived(Guid messageId,DateTime recivedAt)
        {
            var message = await context.Messages.FindAsync(messageId);
            if (message == null)
                throw new ArgumentException($"No message with id: {messageId}");

            message.IsRecived = true;
            message.ReceivedAt = recivedAt;
            await context.SaveChangesAsync();
        }

        public async Task<Message> CreateMessage(Guid senderId, Guid RecipientId, string MessageBody, DateTime SentAt)
        {
            var messageDto = new MessageDto()
            {
                IsRecived = false,
                SenderId = senderId,
                RecipientId = RecipientId,
                MessageBody = MessageBody,
                SentAt = SentAt
            };
            await context.Messages.AddAsync(messageDto);
            await context.SaveChangesAsync();
            return new Message() 
            {
                Id = messageDto.Id,
                IsReceived = messageDto.IsRecived,
                SenderId = messageDto.SenderId,
                RecipientId = messageDto.RecipientId,
                MessageBody = messageDto.MessageBody,
                SentAt = messageDto.SentAt,
                ReceivedAt = messageDto.ReceivedAt
            };
        }

        public Task<IEnumerable<Message>> GetMessages(Guid senderId,Guid recipientId)
        {
            return Task.FromResult<IEnumerable<Message>>(context.Messages.Where(m => m.SenderId == senderId && m.RecipientId == recipientId).Select(m=>new Message
            {
                Id = m.Id,
                MessageBody = m.MessageBody,
                SenderId = m.SenderId,
                RecipientId = m.RecipientId,
                IsReceived = m.IsRecived,
                ReceivedAt = m.ReceivedAt,
                SentAt = m.SentAt
            }).ToList());
        }
    }
}
