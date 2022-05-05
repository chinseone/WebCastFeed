using System.Text.Json.Serialization;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Models.Requests
{
    public class CreateH5ProfileRequest
    {
        [JsonPropertyName("role")]
        public int Role { get; set; }

        public string Items { get; set; }

        public string Title { get; set; }

        public Platform Platform { get; set; }

        public string Nickname { get; set; }

        public long TicketId { get; set; }
    }
}
