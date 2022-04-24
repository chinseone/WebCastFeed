using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models;
using WebCastFeed.Models.Response;

namespace WebCastFeed.Operations
{
    public class ValidateDouyinWebhookOperation : IAsyncOperation<DouyinWebhookValidationModel, DouyinWebhookValidateResponse>
    {
        public async ValueTask<DouyinWebhookValidateResponse> ExecuteAsync(DouyinWebhookValidationModel input, CancellationToken cancellationToken = default)
        {
            if (input?.Content == null)
            {
                return null;
            }

            var content = input.Content;

            return new DouyinWebhookValidateResponse()
            {
               Challenge = content.Challenge
            };
        }
    }
}
