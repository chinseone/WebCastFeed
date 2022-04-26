using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

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

        public async Task UpdateTicket(Ticket ticket)
        {
            await using var transaction = await _XiugouDbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = await _XiugouDbContext.Tickets
                    .SingleOrDefaultAsync(e => e.Id == ticket.Id)
                    .ConfigureAwait(false);
                entity.Event = ticket.Event;
                entity.Platform = ticket.Platform;
                entity.IsDistributed = ticket.IsDistributed;
                entity.IsClaimed = ticket.IsClaimed;
                entity.IsActivated = ticket.IsActivated;
                await _XiugouDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Exception when update ticket status. Exception: {e}");
            }
        }

        public int Save(User user)
        {
            _XiugouDbContext.Users.Add(user);
            return _XiugouDbContext.SaveChanges();
        }

        public async Task<User> GetUserByUserIdAndPlatform(string userId, Platform platform)
        {
            var query = _XiugouDbContext.Users
                .Where(e => e.Platform.Equals(platform))
                .Where(e => e.UserId.Equals(userId));
            return await query.FirstAsync();
        }

        public int Save(Session session)
        {
            _XiugouDbContext.Sessions.Add(session);
            return _XiugouDbContext.SaveChanges();
        }

        public async Task UpdateSessionBySessionId(Session session)
        {
            await using var transaction = await _XiugouDbContext.Database.BeginTransactionAsync();
            try
            {
                var entity = await _XiugouDbContext.Sessions
                    .SingleOrDefaultAsync(e => e.SessionId == session.SessionId)
                    .ConfigureAwait(false);
                entity.IsActive = session.IsActive;
                await _XiugouDbContext.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception e)
            {
                await transaction.RollbackAsync();
                throw new Exception($"Exception when update session status. Exception: {e}");
            }
        }
    }
}
