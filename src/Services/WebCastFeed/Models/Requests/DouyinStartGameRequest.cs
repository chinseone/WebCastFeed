using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class DouyinStartGameRequest
    {
        [JsonPropertyName("accessToken")]
        public string AccessToken { get; set; }

        [JsonPropertyName("anchorId")]
        public string AnchorId { get; set; }
    }
}
