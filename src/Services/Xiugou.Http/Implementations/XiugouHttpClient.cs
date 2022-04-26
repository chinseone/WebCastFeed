﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Xiugou.Http
{
    public class XiugouHttpClient : HttpClient, IXiugouHttpClient
    {
        public XiugouHttpClient(string baseAddress, TimeSpan timeout)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            BaseAddress = new Uri(baseAddress);
            Timeout = timeout;
        }

        public async Task<TOutput> SendAsync<TOutput>(
            HttpMethod httpMethod,
            string path,
            IDictionary<string, object> inputModel = null,
            IDictionary<string, string> headers = null,
            bool returnStringResult = false)
        {
            var cancellationReceiptSource = new CancellationTokenSource();
            var querystring = $"";

            try
            {
                cancellationReceiptSource.CancelAfter(Timeout);

                HttpResponseMessage response;

                if (httpMethod == HttpMethod.Get)
                {
                    if (inputModel != null && inputModel.Any())
                    {
                        querystring += $"?{GetQueryString(inputModel)}";
                    }

                    var task = GetAsync(path + querystring, cancellationReceiptSource.Token);

                    response = await task.ConfigureAwait(false);
                }
                else
                {
                    Console.WriteLine("Request url: " + BaseAddress + path + querystring);
                    var request = CreateRequest(httpMethod, path, querystring, inputModel, headers);
                    response = await SendAsync(
                        request,
                        cancellationReceiptSource.Token).ConfigureAwait(false);
                }

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var unwrappedResponse = JsonConvert.DeserializeObject<TOutput>(content);

                    // var unwrappedResponse = returnStringResult ?
                    //     CastUtil.To<TOutput>(content) :
                    //     JsonConvert.DeserializeObject<TOutput>(content);
                    return unwrappedResponse;
                }
                //TODO: throw specific exception
                throw new Exception();
            }
            catch
            {
                Console.WriteLine("Request url: " + BaseAddress + path + querystring);
                throw;
            }
        }

        private HttpRequestMessage CreateRequest(
            HttpMethod httpMethod,
            string path,
            string querystring,
            IDictionary<string, object> inputModel = null,
            IDictionary<string, string> headers = null)
        {
            var request = new HttpRequestMessage()
            {
                Content = inputModel == null ? GetContent(new { }) : GetContent(inputModel),
                Method = httpMethod,
                RequestUri = new Uri(BaseAddress + path + querystring)
            };

            if (headers == null || headers.Count == 0)
            {
                return request;
            }

            foreach (var header in headers)
            {
                request.Headers.Add(header.Key, header.Value);
            }

            return request;
        }

        private HttpContent GetContent(object o)
        {
            return new StringContent(JsonConvert.SerializeObject(o), Encoding.UTF8, "application/json");
        }

        private string GetQueryString(IDictionary<string, object> queryStringParams)
        {
            var pairsAsString = queryStringParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value.ToString())}");
            return string.Join('&', pairsAsString);
        }
    }
}