using System.Text.Json.Serialization;

namespace Xiugou.Http.Models.Requests
{
    public class StartDouyinGameRequest
    {
        [JsonPropertyName("anchor_id")]
        public string AnchorId { get; set; }

        // In form of an array. e.g. [1,2]
        // 1 - 评论数据
        // 2 - 观众离房数据
        [JsonPropertyName("data_sets")]
        public string DataSets { get; set; }

        [JsonPropertyName("notify_url")]
        public string NotifyUrl { get; set; }
    }
}
