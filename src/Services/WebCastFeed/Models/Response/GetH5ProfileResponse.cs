using System.Text.Json.Serialization;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Models.Response
{
    public class GetH5ProfileResponse
    {
        [JsonPropertyName("identification")]
        public string Identification { get; set; }

        [JsonPropertyName("role")]
        public int Role { get; set; }

        [JsonPropertyName("items")]
        public string Items { get; set; }

        [JsonPropertyName("status")]
        public int Status { get; set; }
    }
}
