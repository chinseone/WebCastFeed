using System;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Http;

namespace WebCastFeed.Operations
{
    public class DouyinStartGameOperation : IAsyncOperation<DouyinStartGameRequest, DouyinStartGameResponse>
    {
        private readonly IDouyinClient _DouyinClient;

        public DouyinStartGameOperation(IDouyinClient douyinClient)
        {
            _DouyinClient = douyinClient ?? throw new ArgumentNullException(nameof(douyinClient));
        }

        public async ValueTask<DouyinStartGameResponse> ExecuteAsync(DouyinStartGameRequest input, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _DouyinClient.StartDouyinGame(
                    accessToken: input.AccessToken, 
                    anchorId:input.AnchorId);

                return new DouyinStartGameResponse()
                {
                    SessionId = response.SessionId ?? ""
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}
