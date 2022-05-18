using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebCastFeed.Enums;
using WebCastFeed.Models.Requests;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Operations
{
    public class CreateH5ProfileOperation : IAsyncOperation<CreateH5ProfileRequest, CreateH5ProfileResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public CreateH5ProfileOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<CreateH5ProfileResponse> ExecuteAsync(CreateH5ProfileRequest input, CancellationToken cancellationToken = default)
        {
            if (!ValidateInput(input, out string field))
            {
                return new CreateH5ProfileResponse()
                {
                    Success = false,
                    Error = ErrorCode.InvalidInput,
                    ErrorMessage = $"{field} is invalid"
                };
            }

            if(await AlreadyExists(input.OpenId))
            {
                return new CreateH5ProfileResponse()
                {
                    Success = false,
                    Error = ErrorCode.Duplicated,
                    ErrorMessage = $"Profile already exists"
                };
            }

            var profile = new H5Profile()
            {
                Id = input.Identification,
                Role = input.Role,
                Items = string.Join(",", input.Items),
                Status = input.Status,
                OpenId = input.OpenId
            };

            await _XiugouRepository.CreateH5Profile(profile);

            return new CreateH5ProfileResponse()
            {
                Success = true
            };
        }

        private bool ValidateInput(CreateH5ProfileRequest input, out string field)
        {
            // Items
            var items = input.Items.Split(",");
            if (items.Length != 2)
            {
                field = "items";
                return false;
            }

            // Title
            field = string.Empty;
            return true;
        }

        private async Task<bool> AlreadyExists(string openId)
        {
            var result = await _XiugouRepository.GetH5ProfileByOpenId(openId);

            return result != null;
        }
    }
}
