using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using Newtonsoft.Json;

namespace Xiugou.Http
{
    public class XiugouHttpClient : HttpClient, IXiugouHttpClient
    {
        private readonly string _ContentType = "application/json";

        public XiugouHttpClient(string baseAddress, TimeSpan timeout)
        {
            if (string.IsNullOrWhiteSpace(baseAddress))
            {
                throw new ArgumentNullException(nameof(baseAddress));
            }

            BaseAddress = new Uri(baseAddress);
            Timeout = timeout;
            DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue(_ContentType));
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
            Console.WriteLine($"Ready to send http request...");
            var sw = new Stopwatch();
            try
            {
                sw.Start();
                var cancelAfterMillSecs = int.Parse(Environment.GetEnvironmentVariable("CancelAfterMillSecs") ?? "10000");
                Console.WriteLine($"Cancel after {cancelAfterMillSecs/1000} seconds");
                cancellationReceiptSource.CancelAfter(cancelAfterMillSecs);

                // ping
                Ping sender = new Ping();
                PingReply reply = sender.Send("webcast.bytedance.com");

                if (reply.Status == IPStatus.Success)
                {
                    Console.WriteLine("Ping successful.");
                }
                else
                {
                    Console.WriteLine("Ping failed");
                }

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
                        cancellationReceiptSource.Token)
                        .ConfigureAwait(false);
                    Console.WriteLine("Successfully sent request..");
                }
                sw.Stop();
                Console.WriteLine($"A success request costs ${sw.ElapsedMilliseconds}");

                var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                if (response.IsSuccessStatusCode)
                {
                    var unwrappedResponse = JsonConvert.DeserializeObject<TOutput>(content);
                    Console.WriteLine("Successfully Deserialized Object");
                    // var unwrappedResponse = returnStringResult ?
                    //     CastUtil.To<TOutput>(content) :
                    //     JsonConvert.DeserializeObject<TOutput>(content);
                    return unwrappedResponse;
                }
                //TODO: throw specific exception
                throw new Exception();
            }
            catch(Exception e)
            {
                sw.Stop();
                Console.WriteLine($"A failed request costs ${sw.ElapsedMilliseconds}");
                Console.WriteLine("Request url: " + BaseAddress + path + querystring);
                Console.WriteLine($"An error has occurred, {e}");
                throw;
            }
            finally
            {
                cancellationReceiptSource.Dispose();
            }
        }

        private HttpRequestMessage CreateRequest(
            HttpMethod httpMethod,
            string path,
            string querystring,
            IDictionary<string, object> inputModel = null,
            IDictionary<string, string> headers = null)
        {
            var httpContent = inputModel == null ? GetContent(new { }) : GetContent(inputModel);
            var request = new HttpRequestMessage()
            {
                Content = httpContent,
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

            Console.WriteLine($"Successfully created request");

            return request;
        }

        private HttpContent GetContent(object o)
        {
            var content = JsonConvert.SerializeObject(o);
            return new StringContent(content, Encoding.UTF8, "application/json");
        }

        private string GetQueryString(IDictionary<string, object> queryStringParams)
        {
            var pairsAsString = queryStringParams.Select(kvp => $"{kvp.Key}={HttpUtility.UrlEncode(kvp.Value.ToString())}");
            return string.Join('&', pairsAsString);
        }
    }
}
