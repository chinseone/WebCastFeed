using System.Threading;
using System.Threading.Tasks;

namespace WebCastFeed
{
    /// <summary>
    /// IAsyncOperation
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IAsyncOperation<in TInput, TOutput>
    {
        /// <summary>
        /// Async Operation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask<TOutput> ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
    }

    /// <summary>
    /// IAsyncOperation without response
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    public interface IAsyncOperation<in TInput>
    {
        /// <summary>
        /// Async Operation
        /// </summary>
        /// <param name="input"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        ValueTask ExecuteAsync(TInput input, CancellationToken cancellationToken = default);
    }
}
