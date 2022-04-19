using System;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Operations
{
    public class CreateTicketOperation : IAsyncOperation<CreateTicketRequest, bool>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public CreateTicketOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<bool> ExecuteAsync(CreateTicketRequest input, CancellationToken cancellationToken = default)
        {
            Enum.TryParse(input.TicketType, true, out TicketType ticketType);

            var ticket = new Ticket()
            {
                Code = input.Code,
                IsActivated = false,
                IsClaimed = false,
                IsDistributed = false,
                TicketType = ticketType
            };

            _XiugouRepository.Save(ticket);
            return true;
        }
    }
}
