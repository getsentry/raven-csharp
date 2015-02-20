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
        private readonly IJsonPacketFactory jsonPacketFactory;


        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient" /> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <param name="jsonPacketFactory">The optional factory that will be used to create the <see cref="JsonPacket" /> that will be sent to Sentry.</param>
        public RavenClient(string dsn, IJsonPacketFactory jsonPacketFactory = null)
            : this(new Dsn(dsn), jsonPacketFactory)
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient" /> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <param name="jsonPacketFactory">The optional factory that will be used to create the <see cref="JsonPacket" /> that will be sent to Sentry.</param>
        /// <exception cref="System.ArgumentNullException">dsn</exception>
        public RavenClient(Dsn dsn, IJsonPacketFactory jsonPacketFactory = null)
        {
            if (dsn == null)
                throw new ArgumentNullException("dsn");

            this.currentDsn = dsn;
            this.jsonPacketFactory = jsonPacketFactory ?? new JsonPacketFactory();

            Logger = "root";
            Timeout = TimeSpan.FromSeconds(5);
        }


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
        /// Gets or sets the timeout value in milliseconds for the HTTP communication with Sentry.
        /// </summary>
        /// <value>
        /// The number of milliseconds to wait before the request times out. The default is 5,000 milliseconds (5 seconds).
        /// </value>
        public TimeSpan Timeout { get; set; }


        /// <summary>
        /// Gets or sets the <see cref="Action"/> to execute if an error occurs when executing
        /// <see cref="CaptureException"/> or <see cref="CaptureMessage"/>.
        /// </summary>
        /// <value>
        /// The <see cref="Action"/> to execute if an error occurs when executing
        /// <see cref="CaptureException"/> or <see cref="CaptureMessage"/>.
        /// </value>
        public Action<Exception> ErrorOnCapture { get; set; }


        /// <summary>
        /// Captures the <see cref="Exception" />.
        /// </summary>
        /// <param name="exception">The <see cref="Exception" /> to capture.</param>
        /// <param name="message">The optional messge to capture. Default: <see cref="Exception.Message" />.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="exception" />. Default: <see cref="ErrorLevel.Error" />.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="exception" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="exception" />.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="exception" />, or <c>null</c> if it fails.
        /// </returns>
        public string CaptureException(Exception exception,
                                       SentryMessage message = null,
                                       ErrorLevel level = ErrorLevel.Error,
                                       IDictionary<string, string> tags = null,
                                       object extra = null)
        {
            try
            {
                JsonPacket packet = this.jsonPacketFactory.Create(this.currentDsn.ProjectID,
                                                                  exception,
                                                                  message,
                                                                  level,
                                                                  tags,
                                                                  extra);
                return Send(packet, CurrentDsn);
            }
            catch (Exception sendException)
            {
                return HandleException(sendException);
            }
        }


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured <paramref name="message" />. Default <see cref="ErrorLevel.Info" />.</param>
        /// <param name="tags">The tags to annotate the captured <paramref name="message" /> with.</param>
        /// <param name="extra">The extra metadata to send with the captured <paramref name="message" />.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured <paramref name="message" />, or <c>null</c> if it fails.
        /// </returns>
        public string CaptureMessage(SentryMessage message,
                                     ErrorLevel level = ErrorLevel.Info,
                                     Dictionary<string, string> tags = null,
                                     object extra = null)
        {
            try
            {
                JsonPacket packet = this.jsonPacketFactory.Create(CurrentDsn.ProjectID, message, level, tags, extra);
                return Send(packet, CurrentDsn);
            }
            catch (Exception sendException)
            {
                return HandleException(sendException);
            }
        }


        private string HandleException(Exception exception)
        {
            try
            {
                if (ErrorOnCapture != null)
                {
                    ErrorOnCapture(exception);
                    return null;
                }

                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(exception);

                WebException webException = exception as WebException;
                if (webException == null || webException.Response == null)
                    return null;

                string messageBody;
                using (Stream stream = webException.Response.GetResponseStream())
                {
                    if (stream == null)
                        return null;

                    using (StreamReader sw = new StreamReader(stream))
                        messageBody = sw.ReadToEnd();
                }

                Console.WriteLine("[MESSAGE BODY] " + messageBody);
            }
            catch (Exception onErrorException)
            {
                Console.WriteLine(onErrorException);
            }

            return null;
        }


        /// <summary>
        /// Sends the specified packet to Sentry.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        protected virtual string Send(JsonPacket packet, Dsn dsn)
        {
            packet.Logger = Logger;

            var request = (HttpWebRequest) WebRequest.Create(dsn.SentryUri);
            request.Timeout = (int) Timeout.TotalMilliseconds;
            request.ReadWriteTimeout = (int) Timeout.TotalMilliseconds;
            request.Method = "POST";
            request.Accept = "application/json";
            request.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
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

            string data = packet.ToString(Formatting.None);

            if (LogScrubber != null)
                data = LogScrubber.Scrub(data);

            // Write the messagebody.
            using (Stream s = request.GetRequestStream())
            {
                if (Compression)
                    GzipUtil.Write(data, s);
                else
                {
                    using (StreamWriter sw = new StreamWriter(s))
                        sw.Write(data);
                }
            }

            using (HttpWebResponse wr = (HttpWebResponse) request.GetResponse())
            using (Stream responseStream = wr.GetResponseStream())
            {
                if (responseStream == null)
                    return null;

                using (StreamReader sr = new StreamReader(responseStream))
                {
                    string content = sr.ReadToEnd();
                    var response = JsonConvert.DeserializeObject<dynamic>(content);
                    return response.id;
                }
            }
        }

        #region Deprecated Methods

        /*
         *  These methods have been deprectaed in favour of the ones
         *  that have the same names as the other sentry clients, this
         *  is purely for the sake of consistency
         */


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to capture.</param>
        /// <returns></returns>
        [Obsolete("The more common CaptureException method should be used")]
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
        [Obsolete("The more common CaptureException method should be used")]
        public string CaptureEvent(Exception e, Dictionary<string, string> tags)
        {
            return CaptureException(e, tags: tags);
        }

        #endregion
    }
}
