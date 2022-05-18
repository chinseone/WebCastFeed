using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class CreateH5ProfileRequest
    {
        [JsonPropertyName("identification")]
        public string Identification { get; set; }

        [JsonPropertyName("role")]
        public int Role { get; set; }

        [JsonPropertyName("items")]
        public string Items { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("openId")]
        public string OpenId { get; set; }
    }
}
