using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Response
{
    public class DouyinWebhookValidateResponse
    {
        [JsonPropertyName("challenge")]
        public int Challenge { get; set; }
    }
}
