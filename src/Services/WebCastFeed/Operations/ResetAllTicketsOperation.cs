using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Operations
{
    public class ResetAllTicketsOperation : IAsyncOperation<bool, ResetAllTicketsResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public ResetAllTicketsOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<ResetAllTicketsResponse> ExecuteAsync(bool input, CancellationToken cancellationToken = default)
        {
            var allTickets = await _XiugouRepository.GetAllTickets();
            var nowUtc = DateTime.UtcNow;

            var count = 0;
            var resultList = new List<GetTicketByCodeResponse>();

            foreach (var ticket in allTickets)
            {
                if (ticket.IsActivated)
                {
                    var currentEvent = ticket.Event.HasValue? ticket.Event.Value : Event.Default;

                    var toTicket = new Ticket()
                    {
                        Id = ticket.Id,
                        Code = ticket.Code,
                        Platform = (Platform)ticket.Platform,
                        Event = (Event)ticket.Event,
                        IsDistributed = true,
                        IsClaimed = false,
                        IsActivated = false,
                        UpdatedUtc = nowUtc,
                        OwnerId = ticket.OwnerId
                    };

                    await _XiugouRepository.UpdateTicket(toTicket);

                    count++;
                    resultList.Add(new GetTicketByCodeResponse()
                    {
                        Code = toTicket.Code,
                        Event = currentEvent.ToString(),
                        IsActivated = toTicket.IsActivated,
                        IsClaimed = toTicket.IsClaimed,
                        IsDistributed = toTicket.IsDistributed,
                        TicketType = toTicket.TicketType,
                        OwnerId = toTicket.OwnerId
                    });
                }
            }

            return new ResetAllTicketsResponse()
            {
                Count = count,
                ImpactedTickets = resultList
            };
        }
    }
}
