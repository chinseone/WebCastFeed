using System;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;
using Xiugou.Http;
using Xiugou.Http.Models.Responses;

namespace WebCastFeed.Operations
{
    public class DouyinStartGameOperation : IAsyncOperation<DouyinStartGameRequest, DouyinStartGameResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;
        private readonly IDouyinClient _DouyinClient;

        public DouyinStartGameOperation(IXiugouRepository xiugouRepository, 
            IDouyinClient douyinClient)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
            _DouyinClient = douyinClient ?? throw new ArgumentNullException(nameof(douyinClient));
        }

        public async ValueTask<DouyinStartGameResponse> ExecuteAsync(DouyinStartGameRequest input, CancellationToken cancellationToken = default)
        {
            try
            {
                bool.TryParse(Environment.GetEnvironmentVariable("UseMockData") ?? "true", out bool useMockData);
                StartDouyinGameResponse response;
                Console.WriteLine($"Use mock data :{useMockData}");
                if (useMockData)
                {
                    response = new StartDouyinGameResponse()
                    {
                        SessionId = Guid.NewGuid().ToString()
                    };
                }
                else
                {
                    response = await _DouyinClient.StartDouyinGame(
                        accessToken: input.AccessToken,
                        anchorId: input.AnchorId);
                }

                if (response.ErrorCode != 0)
                {
                    return new DouyinStartGameResponse()
                    {
                        SessionId = string.Empty,
                        ErrorCode = response.ErrorCode,
                        ErrorMessage = response.ErrorMessage
                    };
                }

                var now = DateTime.UtcNow;
                var session = new Session()
                {
                    AnchorId = input.AnchorId,
                    SessionId = response.SessionId,
                    IsActive = true,
                    CreatedUtc = now,
                    UpdatedUtc = now
                };

                // _XiugouRepository.Save(session);

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
