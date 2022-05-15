using System;
using System.Text.Json.Serialization;
using Xiugou.Entities.Enums;

namespace MessageProcessor.Models
{
    public class ChatMessage
    {
        [JsonPropertyName("platform")]
        public Platform Platform { get; set; }

        [JsonPropertyName("uid")]
        public string UserId { get; set; }

        [JsonPropertyName("nickname")]
        public string Nickname { get; set; }

        [JsonPropertyName("ticket")]
        public string TicketCode { get; set; }

        [JsonPropertyName("pay")]
        public int pay { get; set; }

        [JsonPropertyName("payGuest")]
        public int PayToGuest { get; set; }

        [JsonPropertyName("items")]
        public string Items { get; set; }

        [JsonPropertyName("joinTimestamp")]
        public DateTime JoinTimestamp { get; set; }

        [JsonPropertyName("lastActiveTimestamp")]
        public DateTime LastActiveTimestamp { get; set; }
    }
}
