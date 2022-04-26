using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Xiugou.Http
{
    public interface IXiugouHttpClient
    {
        Task<TOutput> SendAsync<TOutput>(
            HttpMethod httpMethod,
            string path,
            IDictionary<string, object> inputModel = null,
            IDictionary<string, string> headers = null,
            bool returnStringResult = false);
    }
}
