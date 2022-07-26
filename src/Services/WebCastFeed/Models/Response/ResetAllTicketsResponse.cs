using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Response
{
    public class ResetAllTicketsResponse
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }

        [JsonPropertyName("impactedTickets")]
        public List<GetTicketByCodeResponse> ImpactedTickets { get; set; }
    }
}
