using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xiugou.Entities.Entities;

namespace Xiugou.Entities.Implementations
{
    public class XiugouRepository : IXiugouRepository
    {
        private readonly XiugouDbContext _XiugouDbContext;

        public XiugouRepository(XiugouDbContext xiugouDbContext)
        {
            _XiugouDbContext = xiugouDbContext ?? throw new ArgumentNullException(nameof(xiugouDbContext));
        }

        public async Task<Ticket> GetTicketByCode(string code)
        {
            return await _XiugouDbContext.Tickets
                .SingleOrDefaultAsync(t => t.Code.Equals(code))
                .ConfigureAwait(false);
        }

        public int Save(Ticket ticket)
        {
            _XiugouDbContext.Tickets.Add(ticket);
            return _XiugouDbContext.SaveChanges();
        }

        public async Task UpdateIsDistributed(Ticket ticket)
        {
            var entity = await _XiugouDbContext.Tickets
                .SingleOrDefaultAsync(e => e.Code == ticket.Code)
                .ConfigureAwait(false);
            entity.IsDistributed = ticket.IsDistributed;
            _XiugouDbContext.SaveChanges();
        }

        public async Task UpdateIsClaimed(Ticket ticket)
        {
            var entity = await _XiugouDbContext.Tickets
                .SingleOrDefaultAsync(e => e.Code == ticket.Code)
                .ConfigureAwait(false);
            entity.IsClaimed = ticket.IsClaimed;
            _XiugouDbContext.SaveChanges();
        }

        public async Task UpdateIsActivated(Ticket ticket)
        {
            var entity = await _XiugouDbContext.Tickets
                .SingleOrDefaultAsync(e => e.Code == ticket.Code)
                .ConfigureAwait(false);
            entity.IsActivated = ticket.IsActivated;
            _XiugouDbContext.SaveChanges();
        }

        public async Task UpdateTicketState(Ticket ticket)
        {
            var entity = await _XiugouDbContext.Tickets
                .SingleOrDefaultAsync(e => e.Id == ticket.Id)
                .ConfigureAwait(false);
            entity.IsDistributed = ticket.IsDistributed;
            entity.IsClaimed = ticket.IsClaimed;
            entity.IsActivated = ticket.IsActivated;
            _XiugouDbContext.SaveChanges();
        }
    }
}
