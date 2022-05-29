using System.Collections.Generic;
using System.Threading.Tasks;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Entities
{
    public interface IXiugouRepository
    {
        // // Ticket
        Task Save(Ticket ticket);

        Task<Ticket> GetTicketByCode(string code);
        
        Task<List<Ticket>> GetAllTickets();
        
        Task UpdateTicket(Ticket ticket);

        // User
        Task Save(User user);
        
        Task<User> GetUserByUserIdAndPlatform(string userId, Platform platform);

        // H5Profile
        Task CreateH5Profile(H5Profile profile);
        
        Task<H5Profile> GetH5ProfileByOpenId(string openId);

        Task<List<string>> GetAllH5Profiles();
    }
}
