using System.Threading.Tasks;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public interface IXiugouRepository
    {
        int Save(Ticket ticket);

        Task<Ticket> GetTicketByCode(string code);

        Task UpdateTicket(Ticket ticket);

        int Save(User user);

        Task<User> GetUserByUserIdAndPlatform(string userId, Platform platform);

        int Save(Session session);

        Task UpdateSessionBySessionId(Session session);

        Task<Session> GetSessionByAnchorId(string anchorId);
    }
}
