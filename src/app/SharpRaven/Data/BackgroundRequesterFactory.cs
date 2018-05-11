#if !NET40 && !NET35

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

using SharpRaven.Utilities;

namespace SharpRaven.Data
{
    public class BackgroundRequesterFactory : IRequesterFactory, IDisposable
    {
        private readonly IRequesterFactory actualRequesterFactory;
        private readonly BlockingCollection<IRequester> queue;
        private readonly CancellationTokenSource cancellationTokenSource;

        // For testing
        internal Task QueueConsumer { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundRequesterFactory"/> class.
        /// </summary>
        /// <remarks>
        /// Upon instatiation, this class will spawn a Task that will asynchronously send any queued up request to Sentry.
        /// When the queue is empty, the task will sleep asynchronously.
        /// When cancellation is requested, the worker will continue to run until <param name="shutudownTimeout"></param> is reached or the queue is empty.
        /// </remarks>
        /// <param name="actualRequesterFactory">The decorated request factory</param>
        /// <param name="emptyQueueDelay">If the queue is empty, time to asynchonously wait until checking for requests again. Defaults to 1 second</param>
        /// <param name="shutudownTimeout">Time to keep sending events after (blocking Dispose). Defaults to 2 seconds.</param>
        public BackgroundRequesterFactory(
            IRequesterFactory actualRequesterFactory,
            TimeSpan? emptyQueueDelay = null,
            TimeSpan? shutudownTimeout = null,
            int boundedCapacity = 40)
        : this(
            actualRequesterFactory,
            new ConcurrentQueue<IRequester>(),
            new CancellationTokenSource(),
            emptyQueueDelay ?? TimeSpan.FromSeconds(1),
            shutudownTimeout ?? TimeSpan.FromSeconds(2),
            boundedCapacity)
        {
        }

        internal BackgroundRequesterFactory(
            IRequesterFactory actualRequesterFactory,
            IProducerConsumerCollection<IRequester> queue,
            CancellationTokenSource cancellationTokenSource,
            TimeSpan emptyQueueDelay,
            TimeSpan shutudownTimeout,
            int boundedCapacity)
        {
            if (boundedCapacity <= 0) throw new ArgumentOutOfRangeException("At least 1 item must be allowed in the queue.");
            this.actualRequesterFactory = actualRequesterFactory ?? throw new ArgumentNullException(nameof(actualRequesterFactory));
            this.queue = new BlockingCollection<IRequester>(queue, boundedCapacity) ?? throw new ArgumentNullException(nameof(queue));
            this.cancellationTokenSource = cancellationTokenSource ?? throw new ArgumentNullException(nameof(cancellationTokenSource));

            this.QueueConsumer = Task.Run(
                async () => await Worker(
                    this.queue,
                    this.cancellationTokenSource.Token,
                    emptyQueueDelay,
                    shutudownTimeout));
        }

        private static async Task Worker(
            BlockingCollection<IRequester> queue,
            CancellationToken cancellation,
            TimeSpan emptyQueueDelay,
            // The time to keep running, in case there are requests queued up, after cancellation is requested
            TimeSpan shutdownTimeout)
        {
            DateTimeOffset? exitBy = null;

            while (true)
            {
                // If the exit time was not yet set, and the cancellation was signaled, 
                // set the latest we can keep reading off the queue (while there's still stuff to read)
                if (exitBy == null && cancellation.IsCancellationRequested)
                {
                    exitBy = DateTimeOffset.UtcNow.Add(shutdownTimeout);
                }

                if (exitBy != null && DateTimeOffset.UtcNow >= exitBy)
                {
                    break;
                }

                if (queue.TryTake(out var requester))
                {
                    try
                    {
                        await requester.RequestAsync();
                    }
                    catch (Exception exception)
                    {
                        SystemUtil.WriteError(exception);
                    }
                }
                else
                {
                    if (exitBy != null)
                    {
                        // Exit time set and queue is empty. ready to quit.
                        break;
                    }

                    // Queue is empty, wait asynchronously before polling again
                    try
                    {
                        await Task.Delay(emptyQueueDelay, cancellation);
                    }
                    // Cancellation requested, keep going in case there are more items
                    catch (OperationCanceledException)
                    {
                        exitBy = DateTimeOffset.UtcNow.Add(shutdownTimeout);
                    }
                }
            }
        }


        /// <summary>
        /// Creates a <see cref="BackgroundRequester"/> that upon invoked, enqueues the event to a background worker thread
        /// </summary>
        /// <param name="data">The data to send via background worker.</param>
        /// <param name="dsn">The DSN to be used.</param>
        /// <param name="timeout">The timeout of the operation.</param>
        /// <param name="useCompression">Whether to to compress the payload or not.</param>
        /// <returns></returns>
        public IRequester Create(RequestData data, Dsn dsn, TimeSpan timeout, bool useCompression)
        {
            var actualRequester = this.actualRequesterFactory.Create(data, dsn, timeout, useCompression);
            return new BackgroundRequester(actualRequester, this.queue);
        }


        /// <summary>
        /// Creates a <see cref="BackgroundRequester"/> that upon invoked, enqueues the event to a background worker thread
        /// </summary>
        /// <param name="feedback">The feedback to send via background worker.</param>
        /// <param name="dsn">The DSN to be used.</param>
        /// <returns></returns>
        public IRequester Create(SentryUserFeedback feedback, Dsn dsn)
        {
            var actualRequester = this.actualRequesterFactory.Create(feedback, dsn);
            return new BackgroundRequester(actualRequester, this.queue);
        }


        /// <summary>
        /// Stops the background worker and waits for it to empty the queue until 'shutdownTimeout' is reached
        /// </summary>
        /// <inheritdoc />
        public void Dispose()
        {
            // Immediately requests the Worker to stop.
            this.cancellationTokenSource.Cancel();
            try
            {
                // If there's anything in the queue, it'll keep running until 'shutudownTimeout' is reached
                // If the queue is empty it will quit immediatelly
                this.QueueConsumer.GetAwaiter().GetResult();
            }
            catch (Exception exception)
            {
                SystemUtil.WriteError(exception);
            }

            if (this.queue.Count > 0)
            {
                SystemUtil.WriteError($"Worker stopped while {this.queue.Count} were still in the queue.");
            }
        }
    }
}

#endif