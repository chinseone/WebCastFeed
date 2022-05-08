using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class GetH5ProfileRequest
    {
        [JsonPropertyName("platform")]
        public int Platform { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
    }
}
