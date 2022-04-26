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

        public DouyinClient(string baseUrl)
        {
            if (string.IsNullOrEmpty(baseUrl))
            {
                throw new ArgumentNullException("Douyin baseUrl is not set");
            }

            var urlBuilder = new System.Text.StringBuilder();
            urlBuilder.Append(baseUrl.TrimEnd('/'));

            _HttpClient = new XiugouHttpClient(urlBuilder.ToString(), TimeSpan.Parse("00:00:10"));
        }

        public async Task<StartDouyinGameResponse> StartDouyinGame(string accessToken, string anchorId)
        {
            var request = new StartDouyinGameRequest()
            {
                AnchorId = anchorId,
                DataSets = "[1,2]", // hard coded to comments & leave room
                NotifyUrl = "https://apis.xiugou.club/v1/douyin/live-data"
            };

            var json = JsonConvert.SerializeObject(request);
            var inputModel = JsonConvert.DeserializeObject<Dictionary<string, object>>(json);
            var headers = new Dictionary<string, string>()
            {
                {"X-Token", accessToken},
                {"Content-Type", "application/json"}
            };

            var response = await SendAsync<string>(
                HttpMethod.Post,
                "/api/third_party/dance_game/start",
                inputModel,
                headers,
                true).ConfigureAwait(false);

            return JsonConvert.DeserializeObject<StartDouyinGameResponse>(response);
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
