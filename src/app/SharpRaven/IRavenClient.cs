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
#if (!net35)
using System.Threading.Tasks;
#endif

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
        /// Gets or sets the <see cref="Action"/> to execute to manipulate or extract data from
        /// the <see cref="Requester"/> object before it is used in the <see cref="RavenClient.Send"/> method.
        /// </summary>
        /// <value>
        /// The <see cref="Action"/> to execute to manipulate or extract data from the
        /// <see cref="Requester"/> object before it is used in the <see cref="RavenClient.Send"/> method.
        /// </value>
        Func<Requester, Requester> BeforeSend { get; set; }

        /// <summary>
        /// Enable Gzip Compression?
        /// </summary>
        bool Compression { get; set; }

        /// <summary>
        /// The Dsn currently being used to log exceptions.
        /// </summary>
        Dsn CurrentDsn { get; }

        /// <summary>
        /// The environment (e.g. production)
        /// </summary>
        string Environment { get; set; }

        /// <summary>
        /// Not register the <see cref="Breadcrumb"/> for tracking.
        /// </summary>
        bool IgnoreBreadcrumbs { get; set; }

        /// <summary>
        /// Logger. Default is "root"
        /// </summary>
        string Logger { get; set; }

        /// <summary>
        /// Interface for providing a 'log scrubber' that removes
        /// sensitive information from exceptions sent to sentry.
        /// </summary>
        IScrubber LogScrubber { get; set; }

        /// <summary>
        /// Release.
        /// </summary>
        string Release { get; set; }

        /// <summary>
        /// Default tags
        /// </summary>
        IDictionary<string, string> Tags { get; }

        /// <summary>
        /// Gets or sets the timeout value in milliseconds for the <see cref="System.Net.HttpWebRequest.GetResponse()"/>
        /// and <see cref="System.Net.HttpWebRequest.GetRequestStream()"/> methods.
        /// </summary>
        /// <value>
        /// The number of milliseconds to wait before the request times out. The default is 5,000 milliseconds (5 seconds).
        /// Valid values: a nonnegative integer or <see cref="System.Threading.Timeout.Infinite"/>
        /// </value>
        TimeSpan Timeout { get; set; }


        /// <summary>
        /// Captures the <see cref="Breadcrumb"/> for tracking.
        /// </summary>
        /// <param name="breadcrumb">The <see cref="Breadcrumb" /> to capture.</param>
        void AddTrail(Breadcrumb breadcrumb);


        /// <summary>Captures the specified <paramref name="event"/>.</summary>
        /// <param name="event">The event to capture.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="event" />, or <c>null</c> if it fails.
        /// </returns>
        string Capture(SentryEvent @event);


        /// <summary>
        /// Captures the <see cref="Exception" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to capture.</param>
        /// <param name="message">The optional message to capture instead of the default <see cref="Exception.Message" />.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="exception" />. Default: <see cref="ErrorLevel.Error" />.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="exception" /> with.</param>
        /// <param name="fingerprint">The custom fingerprint to identify the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="exception" />.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="exception" />, or <c>null</c> if it fails.
        /// </returns>
        [Obsolete("Use Capture(SentryEvent) instead.")]
        string CaptureException(Exception exception,
                                SentryMessage message = null,
                                ErrorLevel level = ErrorLevel.Error,
                                IDictionary<string, string> tags = null,
                                string[] fingerprint = null,
                                object extra = null);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="message" />. Default <see cref="ErrorLevel.Info" />.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="message" /> with.</param>
        /// <param name="fingerprint">The custom fingerprint to identify the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="message" />.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="message" />, or <c>null</c> if it fails.
        /// </returns>
        [Obsolete("Use Capture(SentryEvent) instead")]
        string CaptureMessage(SentryMessage message,
                              ErrorLevel level = ErrorLevel.Info,
                              IDictionary<string, string> tags = null,
                              string[] fingerprint = null,
                              object extra = null);


        /// <summary>
        /// Restart the capture of the <see cref="Breadcrumb"/> for tracking.
        /// </summary>
        void RestartTrails();


        #if (!net40) && !(net35)
        /// <summary>Captures the event.</summary>
        /// <param name="event">The event to capture.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="event" />, or <c>null</c> if it fails.
        /// </returns>
        Task<string> CaptureAsync(SentryEvent @event);


        /// <summary>
        /// Captures the <see cref="Exception" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to capture.</param>
        /// <param name="message">The optional message to capture instead of the default <see cref="Exception.Message" />.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="exception" />. Default: <see cref="ErrorLevel.Error"/>.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="exception" /> with.</param>
        /// <param name="fingerprint">The custom fingerprint to identify the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="exception" />.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="exception" />, or <c>null</c> if it fails.
        /// </returns>
        [Obsolete("Use CaptureAsync(SentryEvent) instead.")]
        Task<string> CaptureExceptionAsync(Exception exception,
                                           SentryMessage message = null,
                                           ErrorLevel level = ErrorLevel.Error,
                                           IDictionary<string, string> tags = null,
                                           string[] fingerprint = null,
                                           object extra = null);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="message"/>. Default <see cref="ErrorLevel.Info"/>.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="message"/> with.</param>
        /// <param name="fingerprint">The custom fingerprint to identify the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="message"/>.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured <paramref name="message"/>, or <c>null</c> if it fails.
        /// </returns>
        [Obsolete("Use CaptureAsync(SentryEvent) instead.")]
        Task<string> CaptureMessageAsync(SentryMessage message,
                                         ErrorLevel level = ErrorLevel.Info,
                                         IDictionary<string, string> tags = null,
                                         string[] fingerprint = null,
                                         object extra = null);

        #endif

        #region Deprecated Methods

        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <returns></returns>
        [Obsolete("Use CaptureException() instead.", true)]
        string CaptureEvent(Exception e);


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        [Obsolete("Use CaptureException() instead.", true)]
        string CaptureEvent(Exception e, Dictionary<string, string> tags);

        #endregion
    }
}