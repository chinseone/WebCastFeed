using System;
using System.Threading;
using System.Threading.Tasks;

namespace WebCastFeed.WebSocket
{
    internal class AsyncReaderWriterLock : IDisposable
    {
        /// <summary>
        /// Blocks writer processes from entering while at least one other process is holding it.
        /// </summary>
        private readonly SemaphoreSlim _WriteLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Blocks any process from entering while held.
        /// </summary>
        private readonly SemaphoreSlim _ExclusiveLock = new SemaphoreSlim(1, 1);

        /// <summary>
        /// Holds the current count of readers holding the lock.
        /// Must be changed in a thread-safe way!
        /// </summary>
        private int _ReaderCounter;

        /// <summary>
        /// Takes over the lock exclusively and blocks all other processes until finished.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task EnterWriteLockAsync(CancellationToken cancellationToken = default)
        {
            // Need to acquire both locks!
            // This will block any reader/writer from now on, but we are not done...
            await _ExclusiveLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                // ... if there were running readers already, we need to also wait until they release
                // the write lock.
                await _WriteLock.WaitAsync(cancellationToken).ConfigureAwait(false);
            }
            catch
            {
                // Must release the exclusive lock, if acquiring the write lock fails!
                _ExclusiveLock.Release();
                throw;
            }
        }

        /// <summary>
        /// Releases the exclusive lock.
        /// </summary>
        public void ExitWriteLock()
        {
            // Everybody must be waiting on the exclusive lock, so it's safe to release the write one first!
            _WriteLock.Release();
            _ExclusiveLock.Release();
        }


        /// <summary>
        /// Enters the lock as a reader.
        /// Multiple readers can enter the lock as long as no writer is holding it.
        /// Writers will block until all readers have exited.
        /// </summary>
        /// <param name="cancellationToken"></param>
        public async Task EnterReadLockAsync(CancellationToken cancellationToken = default)
        {
            // This will block all other processes for the time necessary to increment the counter
            // and (maybe) take the write lock.
            // It could introduce a small performance penalty if many readers are trying to enter
            // simultaneously!
            await _ExclusiveLock.WaitAsync(cancellationToken).ConfigureAwait(false);

            try
            {
                if (1 == Interlocked.Increment(ref _ReaderCounter))
                {
                    // This will acquire the lock only when the readers count changes from 0 to 1.
                    // It will then only releases when it goes back to 0.
                    await _WriteLock.WaitAsync(cancellationToken).ConfigureAwait(false);
                }
            }
            catch
            {
                // never keep the exclusive lock in case of an error
                _ExclusiveLock.Release();
                throw;
            }

            // the next reader can now enter
            _ExclusiveLock.Release();
        }

        /// <summary>
        ///
        /// </summary>
        public void ExitReadLock()
        {
            if (0 == Interlocked.Decrement(ref _ReaderCounter))
            {
                // Here the counter is 0, but we are still holding the lock:
                // - Writer processes.
                //   Must wait for the write lock, so they are still blocked and will wait until
                //   the Release bellow. However they will be holding the exclusive lock, preventing other
                //   readers/writers to enter
                //
                // - Reader processes.
                //   The first one needs to increment the counter back to 1 and needs to wait to acquire the write lock
                //   until the bellow release is done.
                //   It will also be holding the exclusive lock preventing other readers/writers to enter.
                _WriteLock.Release();
            }
        }

        /// <summary>
        /// Dispose all internal resources used to implement the lock.
        /// </summary>
        public void Dispose()
        {
            _ExclusiveLock.Dispose();
            _WriteLock.Dispose();
        }
    }
}
