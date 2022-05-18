using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using System.Web;
using StackExchange.Redis;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace Xiugou.Entities.Implementations
{
    public class XiugouRepository : IXiugouRepository
    {
        private readonly IConnectionMultiplexer _ConnectionMultiplexer;

        public XiugouRepository(IConnectionMultiplexer connectionMultiplexer)
        {
            _ConnectionMultiplexer = connectionMultiplexer ?? throw new ArgumentNullException(nameof(connectionMultiplexer));
        }

        public async Task<Ticket> GetTicketByCode(string code)
        {
            if (string.IsNullOrEmpty(code) || code.Length != 6)
            {
                throw new ArgumentNullException(nameof(code));
            }

            var encodeCode = HttpUtility.UrlEncode(code);

            var db = _ConnectionMultiplexer.GetDatabase();

            var hEntries = await db.HashGetAllAsync($"tickets:{encodeCode}");
            var ticketEntry = hEntries.ToStringDictionary();

            if (ticketEntry.Count == 0)
            {
                return null;
            }

            var platform = string.IsNullOrEmpty(ticketEntry["platform"]) ? Platform.Default
                : (Platform)Enum.Parse(typeof(Platform), ticketEntry["platform"]);

            var eventType = string.IsNullOrEmpty(ticketEntry["event"]) ? Event.Default
                : (Event) Enum.Parse(typeof(Event), ticketEntry["event"]);

            var ticketType = string.IsNullOrEmpty(ticketEntry["type"]) ? TicketType.Default
                : (TicketType)Enum.Parse(typeof(TicketType), ticketEntry["type"]);

            var isDistributed = int.Parse(ticketEntry["isDistributed"]) != 0;
            var isClaimed = int.Parse(ticketEntry["isClaimed"]) != 0;
            var isActivated = int.Parse(ticketEntry["isActivated"]) != 0;

            var keys = db.HashScan("ticket*");

            return new Ticket()
            {
                Code = ticketEntry["code"],
                Platform = platform,
                Event = eventType,
                TicketType = ticketType,
                IsDistributed = isDistributed,
                IsClaimed = isClaimed,
                IsActivated = isActivated
            };
        }
        
        public async Task Save(Ticket ticket)
        {
            if (ticket == null)
            {
                throw new ArgumentNullException(nameof(ticket));
            }

            var db = _ConnectionMultiplexer.GetDatabase();

            await db.HashSetAsync($"tickets:{ticket.Code}", new []
            {
                new HashEntry("code", ticket.Code),
                new HashEntry("platform", ticket.Platform.HasValue ? ticket.Platform.ToString() : string.Empty),
                new HashEntry("event", ticket.Event.HasValue ? ticket.Event.ToString() : string.Empty),
                new HashEntry("type", (int)ticket.TicketType),
                new HashEntry("isDistributed", ticket.IsDistributed),
                new HashEntry("isClaimed", ticket.IsClaimed),
                new HashEntry("isActivated", ticket.IsActivated)
            });
        }
        
        // public async Task<List<Ticket>> GetAllTickets()
        // {
        //     var db = _ConnectionMultiplexer.GetDatabase();
        //     var keys = await db.HashKeysAsync("tickets*");
        // }
        //

        public async Task UpdateTicket(Ticket ticket)
        {
            var db = _ConnectionMultiplexer.GetDatabase();

            var encodeCode = HttpUtility.UrlEncode(ticket.Code);
            var hashKey = $"tickets:{encodeCode}";
            var hEntries = await db.HashGetAllAsync(hashKey);
            var ticketEntry = hEntries.ToStringDictionary();

            if (ticketEntry.Count == 0)
            {
                return;
            }

            await db.HashSetAsync(hashKey, new[]
            {
                new HashEntry("code", ticket.Code),
                new HashEntry("platform", ticket.Platform.HasValue ? ticket.Platform.ToString() : string.Empty),
                new HashEntry("event", ticket.Event.HasValue ? ticket.Event.ToString() : string.Empty),
                new HashEntry("type", ticketEntry["type"]),
                new HashEntry("isDistributed", ticket.IsDistributed),
                new HashEntry("isClaimed", ticket.IsClaimed),
                new HashEntry("isActivated", ticket.IsActivated)
            });

        }
        //
        // public int Save(User user)
        // {
        //     _XiugouDbContext.Users.Add(user);
        //     return _XiugouDbContext.SaveChanges();
        // }
        //
        // public async Task<User> GetUserByUserIdAndPlatform(string userId, Platform platform)
        // {
        //     return await _XiugouDbContext.Users
        //         .Where(e => e.Platform.Equals(platform))
        //         .SingleOrDefaultAsync(e => e.UserId.Equals(userId));
        // }
        //
        // public int Save(Session session)
        // {
        //     _XiugouDbContext.Sessions.Add(session);
        //     return _XiugouDbContext.SaveChanges();
        // }
        //
        // public async Task UpdateSessionBySessionId(Session session)
        // {
        //     await using var transaction = await _XiugouDbContext.Database.BeginTransactionAsync();
        //     try
        //     {
        //         var entity = await _XiugouDbContext.Sessions
        //             .SingleOrDefaultAsync(e => e.SessionId == session.SessionId)
        //             .ConfigureAwait(false);
        //         entity.IsActive = session.IsActive;
        //         await _XiugouDbContext.SaveChangesAsync();
        //         await transaction.CommitAsync();
        //     }
        //     catch (Exception e)
        //     {
        //         await transaction.RollbackAsync();
        //         throw new Exception($"Exception when update session status. Exception: {e}");
        //     }
        // }
        //
        // public async Task<Session> GetMostRecentActiveSessionByAnchorId(string anchorId)
        // {
        //     return await _XiugouDbContext.Sessions
        //         .Where(s => s.AnchorId.Equals(anchorId))
        //         .Where(s => s.IsActive)
        //         .OrderByDescending(entity => EF.Property<DateTime>(entity, "CreatedUtc"))
        //         .FirstAsync().ConfigureAwait(false);
        // }

        public async Task CreateH5Profile(H5Profile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }
        
            var db = _ConnectionMultiplexer.GetDatabase();
        
            var serialProfile = JsonSerializer.Serialize(profile);
        
           await db.StringSetAsync($"{profile.Platform}:{profile.Nickname}", serialProfile);
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
    }
}
