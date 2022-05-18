using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class GetH5ProfileRequest
    {
        [JsonPropertyName("openId")]
        public string OpenId { get; set; }
    }
}
