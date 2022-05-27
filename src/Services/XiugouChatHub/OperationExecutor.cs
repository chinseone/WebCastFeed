using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace XiugouChatHub
{
    public sealed class OperationExecutor
    {
        private readonly ILogger<OperationExecutor> _logger;

        public OperationExecutor(ILogger<OperationExecutor> logger)
        {
            _logger = logger;
        }

        public async ValueTask<TOutput> ExecuteAsync<TOperation, TInput, TOutput>(TOperation operation, TInput input, CancellationToken cancellationToken = default)
            where TOperation : IAsyncOperation<TInput, TOutput>
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }
            var operationName = typeof(TOperation).Name;
            try
            {
                _logger.LogOperationStarted(operationName);
                var response = await operation.ExecuteAsync(input, cancellationToken);
                _logger.LogOperationSucceed(operationName);
                return response;
            }
            catch (Exception ex)
            {
                _logger.LogOperationFailed(operationName, ex);
                throw;
            }
        }

        public async ValueTask ExecuteAsync<TOperation, TInput>(TOperation operation, TInput input, CancellationToken cancellationToken = default)
            where TOperation : IAsyncOperation<TInput>
        {
            if (operation is null)
            {
                throw new ArgumentNullException(nameof(operation));
            }
            var operationName = typeof(TOperation).Name;
            try
            {
                _logger.LogOperationStarted(operationName);
                await operation.ExecuteAsync(input, cancellationToken);
                _logger.LogOperationSucceed(operationName);
            }
            catch (Exception ex)
            {
                _logger.LogOperationFailed(operationName, ex);
                throw;
            }
        }
    }

    internal static class LogExtension
    {
        private static Action<ILogger, string, Exception> _operationStartedLog = LoggerMessage.Define<string>(LogLevel.Information, new EventId(1, "OperationStart"), "{OperationName} started.");
        private static Action<ILogger, string, Exception> _operationSucceedLog = LoggerMessage.Define<string>(LogLevel.Information, new EventId(2, "OperationSucceed"), "{OperationName} succeed.");
        private static Action<ILogger, string, Exception> _operationFailedLog = LoggerMessage.Define<string>(LogLevel.Error, new EventId(3, "OperationFailed"), "{OperationName} got exception.");

        public static void LogOperationStarted(this ILogger<OperationExecutor> logger, string operationName)
            => _operationStartedLog(logger, operationName, default);

        public static void LogOperationSucceed(this ILogger<OperationExecutor> logger, string operationName)
            => _operationSucceedLog(logger, operationName, default);

        public static void LogOperationFailed(this ILogger<OperationExecutor> logger, string operationName, Exception ex)
            => _operationFailedLog(logger, operationName, ex);
    }
}
