#region License

// Copyright (c) 2014 The Sentry Team and individual contributors.
// All rights reserved.
//
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
//
//     1. Redistributions of source code must retain the above copyright notice, this list of
//        conditions and the following disclaimer.
//
//     2. Redistributions in binary form must reproduce the above copyright notice, this list of
//        conditions and the following disclaimer in the documentation and/or other materials
//        provided with the distribution.
//
//     3. Neither the name of the Sentry nor the names of its contributors may be used to
//        endorse or promote products derived from this software without specific prior written
//        permission.
//
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using SharpRaven.Data;
using SharpRaven.Logging;

namespace SharpRaven
{
    /// <summary>
    /// Empty (no-op) implementation for the Raven Client for use in dependency injection
    /// and other places when a silent operation is needed.
    /// </summary>
    public partial class NoOpRavenClient : IRavenClient
    {
        private readonly Dsn currentDsn;
        private readonly IDictionary<string, string> defaultTags;

        /// <summary>
        /// Initializes a new instance of the <see cref="NoOpRavenClient" /> class.
        /// </summary>
        public NoOpRavenClient()
        {
            currentDsn = new Dsn("http://sentry-dsn.invalid");
            defaultTags = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets or sets the <see cref="Action"/> to execute to manipulate or extract data from
        /// the <see cref="IRequester"/> object before it is used in the <see cref="Send"/> method.
        /// </summary>
        /// <value>
        /// The <see cref="Action"/> to execute to manipulate or extract data from the
        /// <see cref="IRequester"/> object before it is used in the <see cref="Send"/> method.
        /// </value>
        public Func<IRequester, IRequester> BeforeSend { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="Action"/> to execute if an error occurs when executing
        /// <see cref="Capture"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Action"/> to execute if an error occurs when executing <see cref="Capture"/>.
        /// </value>
        public Action<Exception> ErrorOnCapture { get; set; }

        /// <summary>
        /// Enable Gzip Compression?
        /// Defaults to <c>false</c>.
        /// </summary>
        public bool Compression { get; set; }

        /// <summary>
        /// The Dsn currently being used to log exceptions.
        /// </summary>
        public Dsn CurrentDsn
        {
            get { return currentDsn; }
        }

        /// <summary>
        /// Interface for providing a 'log scrubber' that removes
        /// sensitive information from exceptions sent to sentry.
        /// </summary>
        public IScrubber LogScrubber { get; set; }

        /// <summary>
        /// The name of the logger. The default logger name is "root".
        /// </summary>
        public string Logger { get; set; }

        /// <summary>
        /// The version of the application.
        /// </summary>
        public string Release { get; set; }

        /// <summary>
        /// The environment (e.g. production)
        /// </summary>
        public string Environment { get; set; }

        /// <summary>
        /// Default tags sent on all events.
        /// </summary>
        public IDictionary<string, string> Tags
        {
            get { return this.defaultTags; }
        }

        /// <summary>
        /// Gets or sets the timeout value in milliseconds for the HTTP communication with Sentry.
        /// </summary>
        /// <value>
        /// The number of milliseconds to wait before the request times out. The default is 5,000 milliseconds (5 seconds).
        /// </value>
        public TimeSpan Timeout { get; set; }

        /// <summary>
        /// Not register the <see cref="Breadcrumb"/> for tracking.
        /// </summary>
        public bool IgnoreBreadcrumbs { get; set; }


        /// <summary>
        /// Captures the last 100 <see cref="Breadcrumb" />.
        /// </summary>
        /// <param name="breadcrumb">The <see cref="Breadcrumb" /> to capture.</param>
        public void AddTrail(Breadcrumb breadcrumb)
        {
        }


        /// <summary>
        /// Restart the capture of the <see cref="Breadcrumb"/> for tracking.
        /// </summary>
        public void RestartTrails()
        {
        }


        /// <summary>Captures the specified <paramref name="event"/>.</summary>
        /// <param name="event">The event to capture.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="event" />, or <c>null</c> if it fails.
        /// </returns>
        public string Capture(SentryEvent @event)
        {
            return Guid.NewGuid().ToString("n");
        }


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to capture.</param>
        /// <returns></returns>
        [Obsolete("Use CaptureException() instead.", true)]
        public string CaptureEvent(Exception e)
        {
            return CaptureException(e);
        }


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        [Obsolete("Use CaptureException() instead.", true)]
        public string CaptureEvent(Exception e, Dictionary<string, string> tags)
        {
            return CaptureException(e, tags : tags);
        }


        /// <summary>
        /// Captures the <see cref="Exception" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to capture.</param>
        /// <param name="message">The optional message to capture. Default: <see cref="Exception.Message" />.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="exception" />. Default: <see cref="ErrorLevel.Error"/>.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="exception" /> with.</param>
        /// <param name="fingerprint">The custom fingerprint to annotate the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="exception" />.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="exception" />, or <c>null</c> if it fails.
        /// </returns>
        [Obsolete("Use Capture(SentryEvent) instead")]
        public string CaptureException(Exception exception,
                                       SentryMessage message = null,
                                       ErrorLevel level = ErrorLevel.Error,
                                       IDictionary<string, string> tags = null,
                                       string[] fingerprint = null,
                                       object extra = null)
        {
            return Guid.NewGuid().ToString("n");
        }


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="message"/>. Default <see cref="ErrorLevel.Info"/>.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="message"/> with.</param>
        /// <param name="fingerprint">The custom fingerprint to annotate the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="message"/>.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured <paramref name="message"/>, or <c>null</c> if it fails.
        /// </returns>
        [Obsolete("Use Capture(SentryEvent) instead")]
        public string CaptureMessage(SentryMessage message,
                                     ErrorLevel level = ErrorLevel.Info,
                                     IDictionary<string, string> tags = null,
                                     string[] fingerprint = null,
                                     object extra = null)
        {
            return Guid.NewGuid().ToString("n");
        }


        /// <summary>Sends the specified user feedback to Sentry</summary>
        /// <returns>An empty string if succesful, otherwise the server response</returns>
        /// <param name="feedback">The user feedback to send</param>
        public string SendUserFeedback(SentryUserFeedback feedback)
        {
            return string.Empty;
        }
    }
}
