using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Xiugou.Http.Models.Requests;
using Xiugou.Http.Models.Responses;

namespace Xiugou.Http
{
    public class DouyinClient : IDouyinClient
    {
        private readonly IXiugouHttpClient _HttpClient;
        private readonly string _NotifyUrl = "https://apis.xiugou.club/v1/douyin/live-feed";
        private readonly string _StartGamePath = "api/third_party/dance_game/start";
        private readonly string _StopGamePath = "api/third_party/dance_game/stop";
        private readonly string _FeedDataSets = "[1,2]";

        public DouyinClient(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("Douyin baseUrl is not set");
            }

            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(baseUrl.TrimEnd('/'));

            _HttpClient = new XiugouHttpClient(urlBuilder.ToString(), TimeSpan.Parse("00:00:30"));
        }

        public async Task<StartDouyinGameResponse> StartDouyinGame(string accessToken, string anchorId)
        {
            var request = new StartDouyinGameRequest()
            {
                AnchorId = anchorId,
                DataSets = _FeedDataSets, // hard coded to comments & leave room
                NotifyUrl = _NotifyUrl
            };

            var json = JsonConvert.SerializeObject(request);
            var inputModel = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var headers = new Dictionary<string, string>()
            {
                {"X-Token", accessToken}
            };

            var response = await SendAsync<string>(
                HttpMethod.Post,
                _StartGamePath,
                inputModel,
                headers,
                true).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<StartDouyinGameResponse>(response);
        }

        public async Task<StopDouyinGameResponse> StopDouyinGame(string accessToken, string anchorId, string sessionId)
        {
            var request = new StopDouyinGameRequest()
            {
                AnchorId = anchorId,
                SessionId = sessionId
            };

            var json = JsonConvert.SerializeObject(request);
            var inputModel = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var headers = new Dictionary<string, string>()
            {
                {"X-Token", accessToken},
            };

            var response = await SendAsync<string>(
                HttpMethod.Post,
                _StopGamePath,
                inputModel,
                headers,
                true).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<StopDouyinGameResponse>(response);
        }

        public async Task<TOutput> SendAsync<TOutput>(
            HttpMethod method,
            string path,
            IDictionary<string, object> inputModel = null,
            IDictionary<string, string> headers = null,
            bool returnStringResult = false)
        {
            return await _HttpClient.SendAsync<TOutput>(
                httpMethod: method,
                path: path,
                inputModel: inputModel,
                headers: headers,
                returnStringResult: returnStringResult).ConfigureAwait(false);
        }
    }
}
