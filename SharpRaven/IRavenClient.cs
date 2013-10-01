using System;
using System.Collections.Generic;

using SharpRaven.Data;
using SharpRaven.Logging;

namespace SharpRaven
{
    /// <summary>
    /// Raven client interface.
    /// </summary>
    public interface IRavenClient
    {
        /// <summary>
        /// The Dsn currently being used to log exceptions.
        /// </summary>
        Dsn CurrentDsn { get; set; }

        /// <summary>
        /// Interface for providing a 'log scrubber' that removes 
        /// sensitive information from exceptions sent to sentry.
        /// </summary>
        IScrubber LogScrubber { get; set; }

        /// <summary>
        /// Enable Gzip Compression?
        /// Defaults to <c>true</c>.
        /// </summary>
        bool Compression { get; set; }

        /// <summary>
        /// Logger. Default is "root"
        /// </summary>
        string Logger { get; set; }


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <returns></returns>
        int CaptureException(Exception e);


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        int CaptureException(Exception e, IDictionary<string, string> tags = null);


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns></returns>
        int CaptureException(Exception e, IDictionary<string, string> tags = null, object extra = null);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <returns></returns>
        int CaptureMessage(string message);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel"/> of the captured message.</param>
        /// <returns></returns>
        int CaptureMessage(string message, ErrorLevel level);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel"/> of the captured message.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        int CaptureMessage(string message, ErrorLevel level, Dictionary<string, string> tags);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel"/> of the captured message.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns></returns>
        int CaptureMessage(string message,
                           ErrorLevel level = ErrorLevel.Info,
                           Dictionary<string, string> tags = null,
                           object extra = null);


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <returns></returns>
        [Obsolete("The more common CaptureException method should be used")]
        int CaptureEvent(Exception e);


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        [Obsolete("The more common CaptureException method should be used")]
        int CaptureEvent(Exception e, Dictionary<string, string> tags);
    }
}