﻿using System.Text.Json.Serialization;

namespace WebCastFeed.Models.Response
{
    public class DouyinStopGameResponse
    {
        [JsonPropertyName("status")]
        public int Status { get; set; }

        [JsonPropertyName("errcode")]
        public int ErrorCode { get; set; }

        [JsonPropertyName("errmsg")]
        public string ErrorMessage { get; set; }
    }
}
