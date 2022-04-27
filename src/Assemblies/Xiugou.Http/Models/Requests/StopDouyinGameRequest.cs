using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Xiugou.Http.Models.Requests
{
    public class StopDouyinGameRequest
    {
        [JsonProperty("anchor_id")]
        public string AnchorId { get; set; }

        [JsonProperty("session_id")]
        public string SessionId { get; set; }
    }
}
