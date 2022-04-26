using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Response
{
    public class DouyinStartGameResponse
    {
        [JsonPropertyName("session_id")]
        public string SessionId { get; set; }

        [JsonPropertyName("errcode")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("errmsg")]
        public string ErrorMessage { get; set; }
    }
}
