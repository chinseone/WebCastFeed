using System.Text.Json.Serialization;

namespace Xiugou.Http.Models.Responses
{
    public class StopDouyinGameResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("errcode")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("errmsg")]
        public string ErrorMessage { get; set; }
    }
}
