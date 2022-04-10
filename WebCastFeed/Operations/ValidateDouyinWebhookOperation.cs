using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Models;

namespace WebCastFeed.Operations
{
    public class ValidateDouyinWebhookOperation : IAsyncOperation<DouyinWebhookValidationModel, string>
    {
        public async ValueTask<string> ExecuteAsync(DouyinWebhookValidationModel input, CancellationToken cancellationToken = default)
        {
            if (input?.Content == null)
            {
                return string.Empty;
            }

            var content = input.Content;

            return content.Challenge.ToString();
        }
    }
}
