using System.Text.Json.Serialization;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Models.Requests
{
    public class GetH5ProfileRequest
    {
        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }
    }
}
