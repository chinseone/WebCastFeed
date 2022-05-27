using System;
using System.Threading.Tasks;
using MessageProcessor.Interfaces;
using MessageProcessor.Models;
using Xiugou.Entities.Entities;

namespace MessageProcessor.Implementations
{
    public class PersistentService : IPersistentService
    {
        private readonly IXiugouRepository _XiugouRepository;

        public PersistentService(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async Task ProcessChatMessageAsync(ChatMessage chatMessage)
        {
            try
            {
                var user = await _XiugouRepository.GetUserByUserIdAndPlatform(chatMessage.UserId, chatMessage.Platform);

                if (user == null)
                {
                    await CreateNewUser(chatMessage);
                    return;
                }

                await UpdateUserInformation(user, chatMessage);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private async Task CreateNewUser(ChatMessage chatMessage)
        {
            var now = DateTime.UtcNow;
            var user = new User()
            {
                Id = new Guid(),
                Platform = chatMessage.Platform,
                UserId = chatMessage.UserId,
                MessageCount = 1,
                NickName = chatMessage.Nickname,
                TicketCode = chatMessage.TicketCode,
                TotalPay = chatMessage.Pay,
                TotalPayGuest = chatMessage.PayToGuest,
                CreatedUtc = now,
                JoinTimestamp = now,
                LastTimestamp = now
            };

            await _XiugouRepository.Save(user);
        }

        private async Task UpdateUserInformation(User user, ChatMessage chatMessage)
        {
            var now = DateTime.UtcNow;
            var userToUpdate = new User()
            {
                Id = user.Id,
                Platform = chatMessage.Platform,
                UserId = chatMessage.UserId,
                MessageCount = user.MessageCount + 1,
                NickName = chatMessage.Nickname,
                TicketCode = chatMessage.TicketCode,
                TotalPay = user.TotalPay + chatMessage.Pay,
                TotalPayGuest = user.TotalPayGuest + chatMessage.PayToGuest,
                CreatedUtc = user.CreatedUtc,
                JoinTimestamp = user.JoinTimestamp,
                LastTimestamp = now
            };

            await _XiugouRepository.Save(userToUpdate);
        }
    }
}
