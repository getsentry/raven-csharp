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
using System.IO;
using System.Linq;
using System.Net;

using Newtonsoft.Json;

using SharpRaven.Data;
using SharpRaven.Logging;
using SharpRaven.Utilities;

namespace SharpRaven
{
    /// <summary>
    /// The Raven Client, responsible for capturing exceptions and sending them to Sentry.
    /// </summary>
    public partial class RavenClient : IRavenClient
    {
        private readonly Dsn currentDsn;
        private readonly IDictionary<string, string> defaultTags;
        private readonly IJsonPacketFactory jsonPacketFactory;
        private readonly ISentryRequestFactory sentryRequestFactory;
        private readonly ISentryUserFactory sentryUserFactory;

        private readonly CircularBuffer<Breadcrumb> breadcrumbs;
        
        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient" /> class. Sentry
        /// Data Source Name will be read from sharpRaven section in your app.config or
        /// web.config.
        /// </summary>
        /// <param name="jsonPacketFactory">The optional factory that will be used to create the <see cref="JsonPacket" /> that will be sent to Sentry.</param>
        public RavenClient(IJsonPacketFactory jsonPacketFactory = null)
            : this(new Dsn(Configuration.Settings.Dsn.Value), jsonPacketFactory)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient" /> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <param name="jsonPacketFactory">The optional factory that will be used to create the <see cref="JsonPacket" /> that will be sent to Sentry.</param>
        /// <param name="sentryRequestFactory">The optional factory that will be used to create the <see cref="SentryRequest"/> that will be sent to Sentry.</param>
        /// <param name="sentryUserFactory">The optional factory that will be used to create the <see cref="SentryUser"/> that will be sent to Sentry.</param>
        public RavenClient(string dsn,
                           IJsonPacketFactory jsonPacketFactory = null,
                           ISentryRequestFactory sentryRequestFactory = null,
                           ISentryUserFactory sentryUserFactory = null)
            : this(new Dsn(dsn), jsonPacketFactory, sentryRequestFactory, sentryUserFactory)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient" /> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <param name="jsonPacketFactory">The optional factory that will be used to create the <see cref="JsonPacket" /> that will be sent to Sentry.</param>
        /// <param name="sentryRequestFactory">The optional factory that will be used to create the <see cref="SentryRequest"/> that will be sent to Sentry.</param>
        /// <param name="sentryUserFactory">The optional factory that will be used to create the <see cref="SentryUser"/> that will be sent to Sentry.</param>
        /// <exception cref="System.ArgumentNullException">dsn</exception>
        public RavenClient(Dsn dsn,
                           IJsonPacketFactory jsonPacketFactory = null,
                           ISentryRequestFactory sentryRequestFactory = null,
                           ISentryUserFactory sentryUserFactory = null)
        {
            if (dsn == null)
                throw new ArgumentNullException("dsn");

            this.currentDsn = dsn;
            this.jsonPacketFactory = jsonPacketFactory ?? new JsonPacketFactory();
            this.sentryRequestFactory = sentryRequestFactory ?? new SentryRequestFactory();
            this.sentryUserFactory = sentryUserFactory ?? new SentryUserFactory();

            Logger = "root";
            Timeout = TimeSpan.FromSeconds(5);
            this.defaultTags = new Dictionary<string, string>();
            breadcrumbs = new CircularBuffer<Breadcrumb>();
        }


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
            get { return this.currentDsn; }
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
            if (IgnoreBreadcrumbs || breadcrumb == null)
                return;
            
            this.breadcrumbs.Add(breadcrumb);
        }

        /// <summary>
        /// Restart the capture of the <see cref="Breadcrumb"/> for tracking.
        /// </summary>
        public void RestartTrails()
        {
            this.breadcrumbs.Clear();
        }

        /// <summary>Captures the specified <paramref name="event"/>.</summary>
        /// <param name="event">The event to capture.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="event" />, or <c>null</c> if it fails.
        /// </returns>
        public string Capture(SentryEvent @event)
        {
            if (@event == null)
                throw new ArgumentNullException("event");

            @event.Tags = MergeTags(@event.Tags);
            if (!breadcrumbs.IsEmpty())
                @event.Breadcrumbs = breadcrumbs.ToList();

            var packet = this.jsonPacketFactory.Create(CurrentDsn.ProjectID, @event);

            var eventId = Send(packet);
            RestartTrails();

            return eventId;
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
            var @event = new SentryEvent(exception)
            {
                Message = message,
                Level = level,
                Extra = extra,
                Tags = MergeTags(tags),
                Fingerprint = fingerprint
            };

            return Capture(@event);
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
            var @event = new SentryEvent(message)
            {
                Level = level,
                Extra = extra,
                Tags = MergeTags(tags),
                Fingerprint = fingerprint
            };

            return Capture(@event);
        }


        /// <summary>
        /// Performs <see cref="JsonPacket"/> post-processing prior to being sent to Sentry.
        /// </summary>
        /// <param name="packet">The prepared <see cref="JsonPacket"/> which has cleared the creation pipeline.</param>
        /// <returns>The <see cref="JsonPacket"/> which should be sent to Sentry.</returns>
        protected virtual JsonPacket PreparePacket(JsonPacket packet)
        {
            packet.Logger = String.IsNullOrWhiteSpace(packet.Logger)
                            || (packet.Logger == "root" && !String.IsNullOrWhiteSpace(Logger))
                ? Logger
                : packet.Logger;
            packet.User = packet.User ?? this.sentryUserFactory.Create();
            packet.Request = packet.Request ?? this.sentryRequestFactory.Create();
            packet.Release = String.IsNullOrWhiteSpace(packet.Release) ? Release : packet.Release;
            packet.Environment = String.IsNullOrWhiteSpace(packet.Environment) ? Environment : packet.Environment;
            return packet;
        }


        /// <summary>Sends the specified packet to Sentry.</summary>
        /// <param name="packet">The packet to send.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        protected virtual string Send(JsonPacket packet)
        {
            string data = null;
            HttpWebRequest request = null;

            try
            {
                request = CreateWebRequest(packet, out data);

                // Write the messagebody.
                using (var s = request.GetRequestStream())
                {
                    if (Compression)
                        GzipUtil.Write(data, s);
                    else
                    {
                        using (var sw = new StreamWriter(s))
                        {
                            sw.Write(data);
                        }
                    }
                }

                using (var wr = (HttpWebResponse)request.GetResponse())
                {
                    using (var responseStream = wr.GetResponseStream())
                    {
                        if (responseStream == null)
                            return null;

                        using (var sr = new StreamReader(responseStream))
                        {
                            var content = sr.ReadToEnd();
                            var response = JsonConvert.DeserializeObject<dynamic>(content);
                            return response.id;
                        }
                    }
                }
            }
            catch (Exception exception)
            {
                return HandleException(exception, data, request);
            }
        }


        private HttpWebRequest CreateWebRequest(JsonPacket packet, out string data)
        {
            packet = PreparePacket(packet);

            var request = (HttpWebRequest)WebRequest.Create(this.currentDsn.SentryUri);
            request.Timeout = (int)Timeout.TotalMilliseconds;
            request.ReadWriteTimeout = (int)Timeout.TotalMilliseconds;
            request.Method = "POST";
            request.Accept = "application/json";
            request.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(this.currentDsn));
            request.UserAgent = PacketBuilder.UserAgent;

            if (Compression)
            {
                request.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");
                request.AutomaticDecompression = DecompressionMethods.Deflate;
                request.ContentType = "application/octet-stream";
            }
            else
                request.ContentType = "application/json; charset=utf-8";

            /*string data = packet.ToString(Formatting.Indented);
                    Console.WriteLine(data);*/

            data = packet.ToString(Formatting.None);

            if (LogScrubber != null)
                data = LogScrubber.Scrub(data);

            return request;
        }


        private string HandleException(Exception exception, string data, WebRequest request)
        {
            string id = null;

            try
            {
                if (ErrorOnCapture != null)
                {
                    ErrorOnCapture(exception);
                    return null;
                }

                if (exception != null)
                    WriteError(exception.ToString());

                WriteError("Request body:", data);
                WriteError("Request headers:", request.Headers.ToString());

                var webException = exception as WebException;
                if (webException == null || webException.Response == null)
                    return null;

                var response = webException.Response;
                id = response.Headers["X-Sentry-ID"];
                if (String.IsNullOrWhiteSpace(id))
                    id = null;

                string messageBody;
                using (var stream = response.GetResponseStream())
                {
                    if (stream == null)
                        return id;

                    using (var sw = new StreamReader(stream))
                    {
                        messageBody = sw.ReadToEnd();
                    }
                }

                WriteError("Response headers:", response.Headers.ToString());
                WriteError("Response body:", messageBody);
            }
            catch (Exception onErrorException)
            {
                WriteError(onErrorException.ToString());
            }

            return id;
        }


        private static void WriteError(string error)
        {
            if (error == null)
                return;

            Console.ForegroundColor = ConsoleColor.Red;
            Console.Write("[ERROR] ");
            Console.ForegroundColor = ConsoleColor.Gray;
            Console.WriteLine(error);
        }


        private static void WriteError(string description, string multilineData)
        {
            if (multilineData == null)
                return;

            var lines = multilineData.Split(new[] { '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);
            if (lines.Length <= 0)
                return;

            WriteError(description);
            foreach (var line in lines)
            {
                WriteError(line);
            }
        }

        private IDictionary<string, string> MergeTags(IDictionary<string, string> tags = null)
        {
            if (tags == null)
                return this.defaultTags;

            return this.defaultTags
                .Where(kv => !tags.Keys.Contains(kv.Key))
                .Concat(tags)
                .ToDictionary(kv => kv.Key, kv => kv.Value);
        }
    }
}