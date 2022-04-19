using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class CreateTicketRequest
    {
        [JsonPropertyName("code")]
        public string Code { get; set; }

        [JsonPropertyName("type")]
        public string TicketType { get; set; }
    }
}
