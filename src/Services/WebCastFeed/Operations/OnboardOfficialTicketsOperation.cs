using System;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Constants;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Operations
{
    public class OnboardOfficialTicketsOperation : IAsyncOperation<string, bool>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public OnboardOfficialTicketsOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<bool> ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            // Read ticket list and recreate
            var utcNow = DateTime.UtcNow;
            foreach (var ticketPair in OfficialTickets.AllOfficialTickets)
            {
                var ticketCode = ticketPair.Key;
                var ticketType = ticketPair.Value;
                var ticket = new Ticket()
                {
                    Code = ticketCode,
                    TicketType = (TicketType)ticketType,
                    Event = Event.BlueDash,
                    Platform = null,
                    IsDistributed = true,
                    IsClaimed = false,
                    IsActivated = false
                };

                await _XiugouRepository.Save(ticket);
            }

            return true;
        }
    }
}
