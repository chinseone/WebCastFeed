using System.Text.Json.Serialization;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Models.Response
{
    public class GetH5ProfileResponse
    {
        [JsonPropertyName("role")]
        public int Role { get; set; }

        [JsonPropertyName("items")]
        public string Items { get; set; }

        [JsonPropertyName("title")]
        public string Title { get; set; }

        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("ticketId")]
        public long TicketId { get; set; }
    }
}
