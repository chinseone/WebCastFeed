using System;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;
using Xiugou.Http;

namespace WebCastFeed.Operations
{
    public class DouyinStopGameOperation : IAsyncOperation<DouyinStopGameRequest, DouyinStopGameResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;
        private readonly IDouyinClient _DouyinClient;

        public DouyinStopGameOperation(IXiugouRepository xiugouRepository, 
            IDouyinClient douyinClient)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
            _DouyinClient = douyinClient ?? throw new ArgumentNullException(nameof(douyinClient));
        }

        public async ValueTask<DouyinStopGameResponse> ExecuteAsync(DouyinStopGameRequest input, CancellationToken cancellationToken = default)
        {
            try
            {
                var response = await _DouyinClient.StopDouyinGame(input.AccessToken, input.AnchorId, input.SessionId);

                if (response.ErrorCode != 0)
                {
                    return new DouyinStopGameResponse()
                    {
                        Status = response.Status,
                        ErrorCode = response.ErrorCode,
                        ErrorMessage = response.ErrorMessage
                    };
                }

                var session = new Session()
                {
                    AnchorId = input.AnchorId,
                    SessionId = input.SessionId,
                    IsActive = false
                };

                await _XiugouRepository.UpdateSessionBySessionId(session);

                return new DouyinStopGameResponse()
                {
                    Status = 1
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
