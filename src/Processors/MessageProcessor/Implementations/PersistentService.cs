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
            var user = await _XiugouRepository
                .GetUserByPlatformAndUserId(chatMessage.Platform, chatMessage.UserId);

            if (user == null)
            {
                
            }
        }
    }
}
