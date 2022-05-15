using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Response
{
    public class UpdateTicketResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("userId")]
        public string UserId { get; set; }

        [JsonPropertyName("platform")]
        public int Platform { get; set; }

        [JsonPropertyName("ticketCode")]
        public string TicketCode { get; set; }
    }
}
