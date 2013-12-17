#region License

// Copyright (c) 2013 The Sentry Team and individual contributors.
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
    public class RavenClient : IRavenClient
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient"/> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        public RavenClient(string dsn)
            : this(new Dsn(dsn))
        {
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient"/> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        public RavenClient(Dsn dsn)
        {
            CurrentDsn = dsn;
            Compression = true;
            Logger = "root";
        }


        /// <summary>
        /// The Dsn currently being used to log exceptions.
        /// </summary>
        public Dsn CurrentDsn { get; set; }

        /// <summary>
        /// Interface for providing a 'log scrubber' that removes 
        /// sensitive information from exceptions sent to sentry.
        /// </summary>
        public IScrubber LogScrubber { get; set; }

        /// <summary>
        /// Enable Gzip Compression?
        /// Defaults to <c>true</c>.
        /// </summary>
        public bool Compression { get; set; }

        /// <summary>
        /// Logger. Default is "root"
        /// </summary>
        public string Logger { get; set; }


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns>
        /// The ID of the successfully captured <see cref="Exception"/>, or <c>null</c> if it fails.
        /// </returns>
        public string CaptureException(Exception e, IDictionary<string, string> tags = null, object extra = null)
        {
            JsonPacket packet = new JsonPacket(CurrentDsn.ProjectID, e)
            {
                Level = ErrorLevel.Error,
                Tags = tags,
                Extra = extra
            };

            return Send(packet, CurrentDsn);
        }


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured message.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns>
        /// The ID of the successfully captured message, or <c>null</c> if it fails.
        /// </returns>
        public string CaptureMessage(string message,
                                     ErrorLevel level = ErrorLevel.Info,
                                     Dictionary<string, string> tags = null,
                                     object extra = null)
        {
            JsonPacket packet = new JsonPacket(CurrentDsn.ProjectID)
            {
                Message = message,
                Level = level,
                Tags = tags,
                Extra = extra
            };

            return Send(packet, CurrentDsn);
        }


        /// <summary>
        /// Sends the specified packet to Sentry.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <returns>
        /// The ID of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        private string Send(JsonPacket packet, Dsn dsn)
        {
            packet.Logger = Logger;

            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(dsn.SentryUri);
                request.Method = "POST";
                request.Accept = "application/json";
                request.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                request.UserAgent = PacketBuilder.UserAgent;

                if (Compression)
                {
                    request.Headers.Add(HttpRequestHeader.ContentEncoding, "deflate");
                    request.AutomaticDecompression = DecompressionMethods.Deflate;
                    request.ContentType = "application/octet-stream";
                }
                else
                {
                    request.ContentType = "application/json; charset=utf-8";
                }

                string data = packet.ToString();

                if (LogScrubber != null)
                    data = LogScrubber.Scrub(data);

                // Write the message body.
                using (Stream stream = request.GetRequestStream())
                {
                    using (StreamWriter writer = new StreamWriter(stream))
                    {
                        if (Compression)
                            data = GzipUtil.CompressEncode(data);

                        Console.WriteLine(data);

                        writer.Write(data);
                    }
                }

                using (HttpWebResponse wr = (HttpWebResponse) request.GetResponse())
                {
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
            }
            catch (Exception exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(exception);

                WebException webException = exception as WebException;
                if (webException != null && webException.Response != null)
                {
                    string messageBody;
                    using (Stream stream = webException.Response.GetResponseStream())
                    {
                        if (stream == null)
                            return null;

                        using (StreamReader sw = new StreamReader(stream))
                        {
                            messageBody = sw.ReadToEnd();
                        }
                    }

                    Console.WriteLine("[MESSAGE BODY] " + messageBody);
                }
            }

            return null;
        }

        #region Deprecated methods

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
            return CaptureException(e, tags);
        }

        #endregion
    }
}