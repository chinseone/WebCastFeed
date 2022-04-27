using System.Text.Json.Serialization;

namespace WebCastFeed.Models
{
    public class DouyinPayload
    {
        [JsonPropertyName("open_id")]
        public string OpenId { get; set; }

        [JsonPropertyName("content")]
        public string Content { get; set; }

        [JsonPropertyName("avatar_url")]
        public string AvatarUrl { get; set; }

        [JsonPropertyName("nickname")]
        public string NickName { get; set; }

        [JsonPropertyName("timestamp")]
        public long TimeStamp { get; set; }
    }
}
