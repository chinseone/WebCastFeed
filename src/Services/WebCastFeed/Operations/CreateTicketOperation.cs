using System;
using System.Text;
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
        private const string _CharPool = "0123456789abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ";

        public CreateTicketOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<bool> ExecuteAsync(CreateTicketRequest input, CancellationToken cancellationToken = default)
        {
            var parseResult = Enum.TryParse(input.TicketType, true, out TicketType ticketType);

            if (!parseResult)
            {
                return false;
            }

            var code = GenerateTicketCode();
            var tempTicket = await _XiugouRepository.GetTicketByCode(code);

            while (tempTicket != null)
            {
                code = GenerateTicketCode();
                tempTicket = await _XiugouRepository.GetTicketByCode(code);
            }

            var ticket = new Ticket()
            {
                Code = code,
                Event = input.Event,
                IsActivated = false,
                IsClaimed = false,
                IsDistributed = false,
                TicketType = ticketType
            };

            _XiugouRepository.Save(ticket);

            return true;
        }

        private string GenerateTicketCode()
        {
            var len = _CharPool.Length;
            var result = new StringBuilder();

            for(var i = 0; i < 6; i++)
            {
                var r = new Random(len);
                var c = _CharPool[r.Next(0, len)];
                result.Append(c);
            }

            return result.ToString();
        }
    }
}
