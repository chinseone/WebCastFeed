using System;
using System.Threading;
using System.Threading.Tasks;
using WebCastFeed.Constants;
using Xiugou.Entities.Entities;
using Xiugou.Entities.Enums;

namespace WebCastFeed.Operations
{
    public class GetAllH5ProfilesOperation : IAsyncOperation<string, int>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public GetAllH5ProfilesOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<int> ExecuteAsync(string input, CancellationToken cancellationToken = default)
        {
            var allH5s = await _XiugouRepository.GetAllH5Profiles();

            return allH5s.Count;
        }
    }
}
