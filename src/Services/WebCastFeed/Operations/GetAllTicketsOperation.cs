using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;

namespace WebCastFeed.Operations
{
    public class GetAllTicketsOperation : IAsyncOperation<GetAllTicketsRequest, List<GetTicketByCodeResponse>>
    {
        public ValueTask<List<GetTicketByCodeResponse>> ExecuteAsync(GetAllTicketsRequest input, CancellationToken cancellationToken = default)
        {
            if (ValidateInputSignature(input))
            {
                throw new NotImplementedException();
            }
        }

        private bool ValidateInputSignature(GetAllTicketsRequest input)
        {

        }
    }
}
