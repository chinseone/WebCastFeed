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
    public class GetH5ProfileOperation : IAsyncOperation<string, GetH5ProfileResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public GetH5ProfileOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<GetH5ProfileResponse> ExecuteAsync(string openId, CancellationToken cancellationToken = default)
        {
            try
            {
                var profile = await _XiugouRepository.GetH5ProfileByOpenId(openId);
                if (profile == null)
                {
                    return null;
                }

                return new GetH5ProfileResponse()
                {
                    Identification = profile.Id,
                    Role = profile.Role,
                    Items = profile.Items,
                    Status = profile.Status
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
