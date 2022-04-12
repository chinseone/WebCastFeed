using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace WebCastFeed.Models
{
    [DataContract]
    public class WebhookValidationContent
    {
        [JsonPropertyName("challenge")]
        public int Challenge { get; set; }
    }
}
