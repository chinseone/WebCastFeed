using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class DouyinMessage
    {
        [JsonPropertyName("msg_type")]
        public string MessageType { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("payload")]
        public DouyinPayload Payload { get; set; }
    }
}
