using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;

namespace WebCastFeed.Operations
{
    public class GetTicketByCodeOperation : IAsyncOperation<string, GetTicketByCodeResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public GetTicketByCodeOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<GetTicketByCodeResponse> ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            if (!SanitizeInput(input, out var code))
            {
                return null;
            }

            var ticket = await _XiugouRepository.GetTicketByCode(code);

            if (ticket == null)
            {
                return null;
            }
            
            return new GetTicketByCodeResponse()
            {
                Code = ticket.Code,
                Event = ticket.Event.HasValue ? ticket.Event.ToString() : "",
                TicketType = ticket.TicketType,
                IsDistributed = ticket.IsDistributed,
                IsClaimed = ticket.IsClaimed,
                IsActivated = ticket.IsActivated
            };
        }

        private bool SanitizeInput(string input, out string code)
        {
            if (input.All(char.IsLetterOrDigit))
            {
                code = input.ToUpper();
                return true;
            }

            code = string.Empty;
            return false;
        }
    }
}
