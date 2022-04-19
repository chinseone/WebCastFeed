using System.Threading.Tasks;

namespace Xiugou.Entities.Entities
{
    public interface IXiugouRepository
    {
        int Save(Ticket ticket);

        Task<Ticket> GetTicketByCode(string code);

        Task UpdateIsDistributed(Ticket ticket);

        Task UpdateIsClaimed(Ticket ticket);

        Task UpdateIsActivated(Ticket ticket);
    }
}
