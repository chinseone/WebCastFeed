using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;

namespace WebCastFeed.Operations
{
    public class GetAllTicketsOperation : IAsyncOperation<GetAllTicketsRequest, List<GetTicketByCodeResponse>>
    {
        public async ValueTask<List<GetTicketByCodeResponse>> ExecuteAsync(GetAllTicketsRequest input, CancellationToken cancellationToken = default)
        {
            if (ValidateInputSignature(input))
            {
                
            }

            return new List<GetTicketByCodeResponse>();
        }

        private bool ValidateInputSignature(GetAllTicketsRequest input)
        {
            var secret = Environment.GetEnvironmentVariable("GetAllTicketsSecret") ?? "";
            var sigCandidate = $"cmd={input.Command}&timestamp={input.TimeStamp}&secret={secret}";

            var md5 = CreateMd5(sigCandidate);

            var generatedSignature = Convert.ToBase64String(Encoding.UTF8.GetBytes(md5));
            var inputSig = input.Signature;

            return inputSig.Equals(generatedSignature);
        }

        private static string CreateMd5(string input)
        {
            // Use input string to calculate MD5 hash
            using (System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create())
            {
                byte[] inputBytes = System.Text.Encoding.ASCII.GetBytes(input);
                byte[] hashBytes = md5.ComputeHash(inputBytes);

                StringBuilder sb = new System.Text.StringBuilder();
                for (int i = 0; i < hashBytes.Length; i++)
                {
                    sb.Append(hashBytes[i].ToString("X2"));
                }
                return sb.ToString();
            }
        }
    }
}
