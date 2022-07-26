using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;

namespace WebCastFeed.Operations
{
    public class GetAllTicketsOperation : IAsyncOperation<GetAllTicketsRequest, List<GetTicketByCodeResponse>>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public GetAllTicketsOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<List<GetTicketByCodeResponse>> ExecuteAsync(GetAllTicketsRequest input, CancellationToken cancellationToken = default)
        {
            if (ValidateInputSignature(input))
            {
                var allTickets = await _XiugouRepository.GetAllTickets();
            
                return allTickets.Select(t => new GetTicketByCodeResponse()
                {
                    Code = t.Code,
                    Event = t.Event?.ToString(),
                    IsActivated = t.IsActivated,
                    IsClaimed = t.IsClaimed,
                    IsDistributed = t.IsDistributed,
                    TicketType = t.TicketType,
                    OwnerId = t.OwnerId
                }).OrderBy(tt => tt.TicketType).ToList();
            }

            return new List<GetTicketByCodeResponse>();
        }

        private bool ValidateInputSignature(GetAllTicketsRequest input)
        {
            var isGetAllSigValidationEnabled = bool.Parse(Environment.GetEnvironmentVariable("IsGetAllSigValidationEnabled") ?? "false");

            if (!isGetAllSigValidationEnabled)
            {
                return true;
            }
            
            var secret = Environment.GetEnvironmentVariable("GetAllTicketsSecret") ?? "";
            var sigCandidate = $"cmd={input.Command}&timestamp={input.TimeStamp}&secret={secret}";
            
            var md5 = CreateMd5(sigCandidate);
            
            var generatedSignature = Convert.ToBase64String(Encoding.UTF8.GetBytes(md5));
            var inputSig = input.Signature;
            
            return inputSig.Equals(generatedSignature);
        }

        private static string CreateMd5(string input)
        {
            using (var md5 = System.Security.Cryptography.MD5.Create())
            {
                var inputBytes = Encoding.ASCII.GetBytes(input);
                var hashBytes = md5.ComputeHash(inputBytes);

                var sb = new StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
