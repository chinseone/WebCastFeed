using System.Text.Json.Serialization;
using WebCastFeed.Enums;

namespace WebCastFeed.Models.Response
{
    public class CreateH5ProfileResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        [JsonPropertyName("error")]
        public ErrorCode Error { get; set; }

        [JsonPropertyName("err_message")]
        public string ErrorMessage { get; set; }
    }
}
