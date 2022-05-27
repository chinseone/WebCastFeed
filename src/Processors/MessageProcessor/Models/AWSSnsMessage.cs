using System.Text.Json.Serialization;

namespace MessageProcessor.Models
{
    public class AWSSnsMessage
    {
        [JsonPropertyName("Message")]
        public string SerializedRequest { get; set; }
    }
}
