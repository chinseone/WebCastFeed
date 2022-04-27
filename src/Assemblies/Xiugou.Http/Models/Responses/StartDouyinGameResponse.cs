using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Xiugou.Http.Models.Responses
{
    public class StartDouyinGameResponse
    {
        [JsonProperty("session_id")]
        public string SessionId { get; set; }

        [JsonProperty("errcode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }
    }
}
