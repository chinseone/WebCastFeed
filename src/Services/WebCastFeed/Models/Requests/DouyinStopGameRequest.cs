using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class DouyinStopGameRequest
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("anchorId")]
        public string AnchorId { get; set; }

        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }
    }
}
