using System.Threading.Tasks;
using Xiugou.Http.Models.Responses;

namespace Xiugou.Http
{
    public interface IDouyinClient
    {
        Task<StartDouyinGameResponse> StartDouyinGame(string accessToken, string anchor_id);
    }
}
