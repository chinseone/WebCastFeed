using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class DouyinMessage
    {
        [JsonPropertyName("A")]
        public string MessageType { get; set; }
    }
}
