using System;

namespace SharpRaven.Data
{
    /// <summary>
    /// Requester factory
    /// </summary>
    public interface IRequesterFactory
    {
        /// <summary>
        /// Creates a Requester with the specified data.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="dsn">The DSN.</param>
        /// <param name="timeout">The timeout.</param>
        /// <param name="useCompression">if set to <c>true</c> [use compression].</param>
        /// <returns></returns>
        IRequester Create(RequestData data, Dsn dsn, TimeSpan timeout, bool useCompression);
        /// <summary>
        /// Creates a Requester for the specified feedback.
        /// </summary>
        /// <param name="feedback">The feedback.</param>
        /// <param name="dsn">The DSN.</param>
        /// <returns></returns>
        IRequester Create(SentryUserFeedback feedback, Dsn dsn);
    }
}