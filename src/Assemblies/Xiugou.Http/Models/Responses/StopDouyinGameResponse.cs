using System.Text.Json.Serialization;
using Newtonsoft.Json;

namespace Xiugou.Http.Models.Responses
{
    public class StopDouyinGameResponse
    {
        [JsonProperty("status")]
        public int Status { get; set; }

        [JsonProperty("errcode")]
        public int ErrorCode { get; set; }

        [JsonProperty("errmsg")]
        public string ErrorMessage { get; set; }
    }
}
