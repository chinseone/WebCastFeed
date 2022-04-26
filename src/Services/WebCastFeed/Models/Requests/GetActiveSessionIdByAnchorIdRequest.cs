using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Requests
{
    public class GetSessionIdByAnchorIdRequest
    {
        [JsonPropertyName("anchorId")]
        public string AnchorId { get; set; }
    }
}
