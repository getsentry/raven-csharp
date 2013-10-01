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
        /// <param name="e">The decimal.</param>
        /// <returns></returns>
        int CaptureException(Exception e);


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The decimal.</param>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        int CaptureException(Exception e, IDictionary<string, string> tags = null);


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The decimal.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="extra">The extra.</param>
        /// <returns></returns>
        int CaptureException(Exception e, IDictionary<string, string> tags = null, object extra = null);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns></returns>
        int CaptureMessage(string message);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        /// <returns></returns>
        int CaptureMessage(string message, ErrorLevel level);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        int CaptureMessage(string message, ErrorLevel level, Dictionary<string, string> tags);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <param name="level">The level.</param>
        /// <param name="tags">The tags.</param>
        /// <param name="extra">The extra.</param>
        /// <returns></returns>
        int CaptureMessage(string message,
                           ErrorLevel level = ErrorLevel.Info,
                           Dictionary<string, string> tags = null,
                           object extra = null);


        /// <summary>
        /// Sends the specified packet.
        /// </summary>
        /// <param name="packet">The packet.</param>
        /// <param name="dsn">The DSN.</param>
        /// <returns></returns>
        bool Send(JsonPacket packet, Dsn dsn);


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The decimal.</param>
        /// <returns></returns>
        [Obsolete("The more common CaptureException method should be used")]
        int CaptureEvent(Exception e);


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The decimal.</param>
        /// <param name="tags">The tags.</param>
        /// <returns></returns>
        [Obsolete("The more common CaptureException method should be used")]
        int CaptureEvent(Exception e, Dictionary<string, string> tags);
    }
}