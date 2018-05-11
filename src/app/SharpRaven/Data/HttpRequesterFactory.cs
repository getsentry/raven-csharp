using System;

namespace SharpRaven.Data
{
    /// <summary>
    /// Factory for Requester that when invoked fired an HTTP request to Sentry
    /// </summary>
    /// <seealso cref="T:SharpRaven.Data.IRequesterFactory" />
    /// <inheritdoc />
    public class HttpRequesterFactory : IRequesterFactory
    {
        /// <summary>
        /// Creates a Requester that initiates an HTTP request upon invoking
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="dsn">The DSN.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="useCompression">if set to <c>true</c> [use compression].</param>
        /// <returns></returns>
        public IRequester Create(
            RequestData data,
            Dsn dsn,
            TimeSpan timeout,
            bool useCompression)
            => new HttpRequester(data, dsn, timeout, useCompression);

        /// <summary>
        /// Creates a Requester that initiates an HTTP request when invoked.
        /// </summary>
        /// <param name="feedback">The feedback.</param>
        /// <param name="dsn">The DSN.</param>
        /// <returns></returns>
        public IRequester Create(
            SentryUserFeedback feedback,
            Dsn dsn)
            => new HttpRequester(feedback, dsn);
    }

}
