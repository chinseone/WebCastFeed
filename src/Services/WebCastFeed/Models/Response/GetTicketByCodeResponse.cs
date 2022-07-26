using System.Text.Json.Serialization;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Models.Response
{
    public class GetTicketByCodeResponse
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("eventType")]
        public string Event { get; set; }

        [JsonPropertyName("ticketType")]
        public TicketType TicketType { get; set; }

        [JsonPropertyName("isDistributed")]
        public bool IsDistributed { get; set; }

        [JsonPropertyName("isClaimed")]
        public bool IsClaimed { get; set; }

        [JsonPropertyName("isActivated")]
        public bool IsActivated { get; set; }

        [JsonPropertyName("ownerId")]
        public string OwnerId { get; set; }
    }
}
