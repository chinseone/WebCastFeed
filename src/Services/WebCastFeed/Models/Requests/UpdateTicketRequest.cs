using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class UpdateTicketRequest
    {
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [JsonPropertyName("platform")]
        public int Platform { get; set; }

        [JsonPropertyName("event")]
        public int Event { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("isDistributed")]
        public bool IsDistributed { get; set; }

        [JsonPropertyName("isClaimed")]
        public bool IsClaimed { get; set; }

        [JsonPropertyName("isActivated")]
        public bool IsActivated { get; set; }
    }
}
