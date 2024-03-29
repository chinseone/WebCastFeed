﻿using System;
using System.Threading;
using System.Threading.Tasks;
using System.Web;
using WebCastFeed.Models.Response;
using Xiugou.Entities.Entities;

namespace WebCastFeed.Operations
{
    public class GetActiveSessionIdByAnchorIdOperation : IAsyncOperation<string, GetActiveSessionIdByAnchorIdResponse>
    {
        private readonly IXiugouRepository _XiugouRepository;

        public GetActiveSessionIdByAnchorIdOperation(IXiugouRepository xiugouRepository)
        {
            _XiugouRepository = xiugouRepository ?? throw new ArgumentNullException(nameof(xiugouRepository));
        }

        public async ValueTask<GetActiveSessionIdByAnchorIdResponse> ExecuteAsync(string anchorId, CancellationToken cancellationToken = default)
        {
            var sanitizedAnchorId = HttpUtility.UrlEncode(anchorId); // in case of SQL injection

            return null;
            // var session = await _XiugouRepository.GetMostRecentActiveSessionByAnchorId(sanitizedAnchorId);
            //
            // if (session == null)
            // {
            //     return null;
            // }
            //
            // return new GetActiveSessionIdByAnchorIdResponse()
            // {
            //     SessionId = session.SessionId
            // };
        }
    }
}
