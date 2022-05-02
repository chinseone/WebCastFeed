using Newtonsoft.Json;

namespace WebCastFeed.Models.Requests
{
    public class GetAllTicketsRequest
    {
        [JsonProperty("cmd")]
        public string Command { get; set; }

        [JsonProperty("timestamp")]
        public long TimeStamp { get; set; }

        [JsonProperty("signature")]
        public string Signature { get; set; }
    }
}
