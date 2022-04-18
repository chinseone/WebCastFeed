using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebCastFeed.Models
{
    public class DouyinWebhookValidationModel
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }
        
        [JsonPropertyName("client_key")]
        public string ClientKey { get; set; }

        [JsonPropertyName("content")]
        public WebhookValidationContent Content { get; set; }
    }
}
