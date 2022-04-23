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
        private const string _CharPool = "3ghijkl67pqras1tnucvw4xyzAB8C0DEFmGHbIJK5LMNfdPQRS2TUVeWX9YZ";

        public CreateTicketOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<bool> ExecuteAsync(CreateTicketRequest input, CancellationToken cancellationToken = default)
        {
            var ticketTypeParseResult = Enum.TryParse(input.TicketType, true, out TicketType ticketType);

            if (!ticketTypeParseResult)
            {
                return false;
            }

            var code = GenerateTicketCode();
            try
            {
                var tempTicket = await _XiugouRepository.GetTicketByCode(code);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
            

            // while (tempTicket != null)
            // {
            //     code = GenerateTicketCode();
            //     tempTicket = await _XiugouRepository.GetTicketByCode(code);
            // }

            var ticket = new Ticket()
            {
                Code = code,
                TicketType = ticketType,
                Event = null,
                Platform = null,
                IsActivated = false,
                IsClaimed = false,
                IsDistributed = false
            };

            _XiugouRepository.Save(ticket);

            return true;
        }

        private string GenerateTicketCode()
        {
            var len = _CharPool.Length;
            var result = new StringBuilder();
            var r = new Random(len);

            for (var i = 0; i < 6; i++)
            {
                var c = _CharPool[r.Next(0, len)];
                result.Append(c);
            }

            return result.ToString();
        }
    }
}
