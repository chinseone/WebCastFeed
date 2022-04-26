using System.Text.Json.Serialization;

namespace Xiugou.Http.Models.Requests
{
    public class StopDouyinGameRequest
    {
        [JsonPropertyName("anchor_id")]
        public string AnchorId { get; set; }

        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }
    }
}
