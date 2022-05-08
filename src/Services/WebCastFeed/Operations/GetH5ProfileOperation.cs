using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

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
            try
            {
                var plat = (Platform)input.Platform;
                var nickname = HttpUtility.UrlEncode(Encoding.UTF8.GetBytes(input.Nickname));
                var result = await _XiugouRepository.GetH5ProfileByPlatformAndNickname(plat, nickname);
                if (result == null)
                {
                    return null;
                }

                return new GetH5ProfileResponse()
                {
                    Items = result.Items,
                    Nickname = HttpUtility.UrlDecode(nickname),
                    Platform = result.Platform,
                    Role = result.Role,
                    TicketId = result.TicketId,
                    Title = result.Title
                };
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return null;
            }
        }
    }
}
