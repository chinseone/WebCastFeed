using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Response
{
    public class GetActiveSessionIdByAnchorIdResponse
    {
        [JsonPropertyName("sessionId")]
        public string SessionId { get; set; }
    }
}
