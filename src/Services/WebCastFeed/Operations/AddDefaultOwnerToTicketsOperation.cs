using System;
using System.Threading;
using System.Threading.Tasks;
using Xiugou.Entities.Entities;

namespace WebCastFeed.Operations
{
    public class AddDefaultOwnerToTicketsOperation : IAsyncOperation<string, string>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public AddDefaultOwnerToTicketsOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<string> ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            var allTickets = await _XiugouRepository.GetAllTickets();

            foreach (var ticket in allTickets)
            {
                await _XiugouRepository.AddDefaultOwnerIdToTicket(ticket);
            }

            return "done";
        }
    }
}
