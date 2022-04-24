using System.Text.Json.Serialization;

namespace Xiugou.Http.Models.Responses
{
    public class StartDouyinGameResponse
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("errcode")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("errmsg")]
        public string ErrorMessage { get; set; }
    }
}
