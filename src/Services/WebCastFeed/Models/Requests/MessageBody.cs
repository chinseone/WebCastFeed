using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class MessageBody
    {
        [JsonPropertyName("platform")]
        public int Platform { get; set; }

        [JsonPropertyName("uid")]
        public string UserId { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("pay")]
        public int Pay { get; set; }

        [JsonPropertyName("payGuest")]
        public int PayGuest { get; set; }

        [JsonPropertyName("items")]
        public List<int> Items { get; set; }
    }
}
