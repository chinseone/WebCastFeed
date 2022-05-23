﻿using System;
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

# region Ticket
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
        
        public async Task<List<Ticket>> GetAllTickets()
        {
            var result = new List<Ticket>();
            var db = _ConnectionMultiplexer.GetDatabase();
            int nextCursor = 0;
            do
            {
                var redisResult = await db.ExecuteAsync("SCAN", new object[] { nextCursor.ToString(), "MATCH", "tickets*", "COUNT", "1000" });
                var innerResult = (RedisResult[])redisResult;

                nextCursor = int.Parse((string)innerResult[0]);
                List<RedisKey> resultLines = ((RedisKey[])innerResult[1]).ToList();
                foreach (var key in resultLines)
                {
                    var ticketCode = ((string)key).Split(":")[1];
                    var ticket = await GetTicketByCode(ticketCode);
                    result.Add(ticket);
                }
            }
            while (nextCursor != 0);

            return result;
        }
        

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
        #endregion

#region User
        public async Task Save(User user)
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            var db = _ConnectionMultiplexer.GetDatabase();

            await db.HashSetAsync($"users:{user.Platform}:{user.UserId}", new[]
            {
                new HashEntry("userId", user.UserId),
                new HashEntry("platform", user.Platform.ToString()),
                new HashEntry("nickname", user.NickName),
                new HashEntry("ticketCode", user.TicketCode),
                new HashEntry("messageCount", user.MessageCount),
                new HashEntry("totalPay", user.TotalPay),
                new HashEntry("totalPayGuest", user.TotalPayGuest),
                new HashEntry("joinedTime", user.JoinTimestamp.Ticks),
                new HashEntry("lastActiveTime", user.LastTimestamp.Ticks)
            });
        }

        public async Task<User> GetUserByUserIdAndPlatform(string userId, Platform platform)
        {
            if (string.IsNullOrEmpty(userId))
            {
                throw new ArgumentNullException(nameof(userId));
            }

            var db = _ConnectionMultiplexer.GetDatabase();

            var hEntries = await db.HashGetAllAsync($"users:{platform}:{userId}");
            var userEntry = hEntries.ToStringDictionary();

            if (userEntry.Count == 0)
            {
                return null;
            }

            return new User()
            {
                UserId = userId,
                Platform = platform,
                NickName = userEntry["nickname"],
                TicketCode = userEntry["ticketCode"],
                MessageCount = int.Parse(userEntry["messageCount"]),
                TotalPay = int.Parse(userEntry["totalPay"]),
                TotalPayGuest = int.Parse(userEntry["totalPayGuest"]),
                JoinTimestamp = new DateTime(long.Parse(userEntry["joinedTime"])),
                LastTimestamp = new DateTime(long.Parse(userEntry["lastActiveTime"]))
            };
        }
#endregion

#region H5
        public async Task CreateH5Profile(H5Profile profile)
        {
            if (profile == null)
            {
                throw new ArgumentNullException(nameof(profile));
            }

            var db = _ConnectionMultiplexer.GetDatabase();

            var serialProfile = JsonSerializer.Serialize(profile);

            await db.StringSetAsync($"h5:{profile.OpenId}", serialProfile);
        }

        public async Task<H5Profile> GetH5ProfileByOpenId(string openId)
        {
            var db = _ConnectionMultiplexer.GetDatabase();

            var profile = await db.StringGetAsync($"h5:{openId}");

            if (!string.IsNullOrEmpty(profile))
            {
                return JsonSerializer.Deserialize<H5Profile>(profile);
            }

            return null;
        }
#endregion
    }
}
