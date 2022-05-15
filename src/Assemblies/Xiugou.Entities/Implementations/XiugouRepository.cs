using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Implementations
{
    public class XiugouRepository : IXiugouRepository
    {
        private readonly XiugouDbContext _XiugouDbContext;
        private readonly IConnectionMultiplexer _ConnectionMultiplexer;

        public XiugouRepository(XiugouDbContext xiugouDbContext, IConnectionMultiplexer connectionMultiplexer)
        {
            _XiugouDbContext = xiugouDbContext ?? throw new ArgumentNullException(nameof(xiugouDbContext));
            _ConnectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
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

        public async Task<List<Ticket>> GetAllTickets()
        {
            return await _XiugouDbContext.Tickets.ToListAsync();
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

        public async Task<Session> GetMostRecentActiveSessionByAnchorId(string anchorId)
        {
            return await _XiugouDbContext.Sessions
                .Where(s => s.AnchorId.Equals(anchorId))
                .Where(s => s.IsActive)
                .OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedUtc"))
                .FirstAsync().ConfigureAwait(false);
        }

        public async Task CreateH5Profile(H5Profile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var db = _ConnectionMultiplexer.GetDatabase();

            var serialProfile = JsonSerializer.Serialize(profile);

            db.StringSet($"{profile.Platform}:{profile.Nickname}", serialProfile);
        }

        public async Task<H5Profile> GetH5ProfileByPlatformAndNickname(Platform platform, string nickname)
        {
            var db = _ConnectionMultiplexer.GetDatabase();

            var profile = await db.StringGetAsync($"{platform}:{nickname}");

            if (!string.IsNullOrEmpty(profile))
            {
                return JsonSerializer.Deserialize<H5Profile>(profile);
            }

            return null;
        }

        public async Task CreateUser(Platform platform, string userId, string username)
        {
            var db = _ConnectionMultiplexer.GetDatabase();

            await db.HashSetAsync($"{platform}:{userId}", new []
            {
                new HashEntry("userId", userId),
                new HashEntry("platform", platform.ToString()),
                new HashEntry("nickname", username),
                new HashEntry("ticketId", 0),
                new HashEntry("messageCount", 1),
                new HashEntry("totalPay", 0),
                new HashEntry("totalPayGuest", 0),
                new HashEntry("joinTimestamp", DateTime.UtcNow.ToString()),
                new HashEntry("lastTimestamp", DateTime.UtcNow.ToString()),
            });
        }

        public async Task<User> GetUserByPlatformAndUserId(Platform platform, string userId)
        {
            var db = _ConnectionMultiplexer.GetDatabase();

            // var profile = await db.StringGetAsync($"{platform}:{userId}");
            var user = await db.HashGetAsync($"{platform}:{userId}", 
                new RedisValue[]
                {
                    "userId",
                    "platform",
                    "nickname", 
                    "ticketId",
                    "messageCount",
                    "totalPay",
                    "totalPayGuest",
                    "joinTimestamp",
                    "lastTimestamp"
                });

            if (user != null)
            {
                return new User()
                {
                    UserId = user[0],
                    Platform = (Platform)((int)user[1]),
                    NickName = user[2],
                    TicketId = (int)user[3],
                    MessageCount = (int)user[4],
                    TotalPay = (int)user[5],
                    TotalPayGuest = (int)user[6],
                    JoinTimestamp = DateTime.Parse(user[7]),
                    LastTimestamp = DateTime.Parse(user[8]),
                };
            }

            return null;
        }
    }
}
