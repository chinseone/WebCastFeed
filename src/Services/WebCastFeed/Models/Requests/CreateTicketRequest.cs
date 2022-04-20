using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class CreateTicketRequest
    {
        [JsonPropertyName("type")]
        public string TicketType { get; set; }

        [JsonPropertyName("event")]
        public string Event { get; set; }
    }
}
