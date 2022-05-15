using System.Threading.Tasks;
using MessageProcessor.Models;

namespace MessageProcessor.Interfaces
{
    public interface IPersistentService
    {
        Task ProcessChatMessageAsync(ChatMessage chatMessage);
    }
}
