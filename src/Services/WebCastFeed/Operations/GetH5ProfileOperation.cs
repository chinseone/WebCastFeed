using System;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;

namespace WebCastFeed.Operations
{
    public class GetH5ProfileOperation : IAsyncOperation<GetH5ProfileRequest, GetH5ProfileResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public GetH5ProfileOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<GetH5ProfileResponse> ExecuteAsync(GetH5ProfileRequest input, CancellationToken cancellationToken = default)
        {
            var result = await _XiugouRepository.GetH5ProfileByPlatformAndNickname(input.Platform, input.Nickname);

            return new GetH5ProfileResponse()
            {
                Items = result.Items,
                Nickname = result.Nickname,
                Platform = result.Platform,
                Role = result.Role,
                TicketId = result.TicketId,
                Title = result.Title
            };
        }
    }
}
