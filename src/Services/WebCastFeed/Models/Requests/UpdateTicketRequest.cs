using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class UpdateTicketRequest
    {
        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }

        [JsonPropertyName("isDistributed")]
        public bool IsDistributed { get; set; }

        [JsonPropertyName("isClaimed")]
        public bool IsClaimed { get; set; }

        [JsonPropertyName("isActivated")]
        public bool IsActivated { get; set; }
    }
}
