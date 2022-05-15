using System.Collections.Generic;
using System.Threading.Tasks;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public interface IXiugouRepository
    {
        // Ticket
        int Save(Ticket ticket);

        Task<Ticket> GetTicketByCode(string code);

        Task<List<Ticket>> GetAllTickets();

        Task UpdateTicket(Ticket ticket);

        // User
        int Save(User user);

        Task<User> GetUserByUserIdAndPlatform(string userId, Platform platform);

        // Session
        int Save(Session session);

        Task UpdateSessionBySessionId(Session session);

        Task<Session> GetMostRecentActiveSessionByAnchorId(string anchorId);

        // H5Profile
        // Task CreateH5Profile(H5Profile profile);
        //
        // Task<H5Profile> GetH5ProfileByPlatformAndNickname(Platform platform, string nickname);
    }
}
