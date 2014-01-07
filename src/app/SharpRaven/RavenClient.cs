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

#if !(net40)
  using System.Net.Http;
  using System.Net.Http.Headers;
  using System.Threading.Tasks;
#endif

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
        private readonly Dsn currentDsn;


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
            if (dsn == null)
                throw new ArgumentNullException("dsn");

            this.currentDsn = dsn;
            Logger = "root";
        }


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
        /// Enable Gzip Compression?
        /// Defaults to <c>false</c>.
        /// </summary>
        [Obsolete("GZip compression isn't supported yet.", true)]
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
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured <see cref="Exception"/>, or <c>null</c> if it fails.
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
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured message, or <c>null</c> if it fails.
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
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        private string Send(JsonPacket packet, Dsn dsn)
        {
            packet.Logger = Logger;

            try
            {
                HttpWebRequest request = (HttpWebRequest) WebRequest.Create(dsn.SentryUri);
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                request.UserAgent = PacketBuilder.UserAgent;

                // Write the messagebody.
                using (Stream s = request.GetRequestStream())
                {
                    using (StreamWriter sw = new StreamWriter(s))
                    {
                        // Compress and encode.
                        //string data = Utilities.GzipUtil.CompressEncode(packet.Serialize());
                        //Console.WriteLine("Writing: " + data);
                        // Write to the JSON script when ready.
                        string data = packet.ToString();
                        if (LogScrubber != null)
                            data = LogScrubber.Scrub(data);

                        sw.Write(data);
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
            catch (WebException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(e);

                if (e.Response != null)
                {
                    string messageBody;
                    using (Stream stream = e.Response.GetResponseStream())
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

                return null;
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(e);

                return null;
            }
        }

#if !(net40)

        #region async Task<string> CaptureMessageAsync(string message, ErrorLevel level = ErrorLevel.Info, Dictionary<string, string> tags = null, object extra = null)
        /// <summary>
        /// Captures the message asynchronously.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured message.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured message, or <c>null</c> if it fails.
        /// </returns>
        public async Task<string> CaptureMessageAsync(string message, ErrorLevel level = ErrorLevel.Info, Dictionary<string, string> tags = null, object extra = null)
        {
            JsonPacket packet = new JsonPacket(CurrentDsn.ProjectID)
            {
                Message = message,
                Level = level,
                Tags = tags,
                Extra = extra
            };


            return await SendAsync(packet, CurrentDsn);
        }
        #endregion

        #region async Task<string> CaptureExceptionAsync(Exception e, IDictionary<string, string> tags = null, object extra = null)
        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured <see cref="Exception"/>, or <c>null</c> if it fails.
        /// </returns>
        public async Task<string> CaptureExceptionAsync(Exception e, IDictionary<string, string> tags = null, object extra = null)
        {
            JsonPacket packet = new JsonPacket(CurrentDsn.ProjectID, e)
            {
                Level = ErrorLevel.Error,
                Tags = tags,
                Extra = extra
            };

            return await SendAsync(packet, CurrentDsn);
        }
        #endregion

        #region async Task<string> SendAsync(JsonPacket packet, Dsn dsn)
        /// <summary>
        /// Sends the specified packet to Sentry asynchronously.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        private async Task<string> SendAsync(JsonPacket packet, Dsn dsn)
        {
            string data, responseContent;
            HttpClient httpClient;
            HttpContent httpContent;
            HttpResponseMessage httpResponse;
            StringContent stringContent;

            packet.Logger = Logger;

            try
            {
                data = packet.ToString();
                if (LogScrubber != null)
                {
                    data = LogScrubber.Scrub(data);
                }

                using (httpClient = new HttpClient())
                {
                    httpClient.DefaultRequestHeaders.Accept.Add(MediaTypeWithQualityHeaderValue.Parse("application/json"));
                    httpClient.DefaultRequestHeaders.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
                    httpClient.DefaultRequestHeaders.UserAgent.Add(new ProductInfoHeaderValue("RavenSharp", "1.0"));

                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;

                    using (stringContent = new StringContent(data))
                    {
                        stringContent.Headers.ContentType.CharSet = "utf-8";
                        stringContent.Headers.ContentType.MediaType = "application/json";

                        using (httpResponse = await httpClient.PostAsync(dsn.SentryUri, stringContent))
                        using (httpContent = httpResponse.Content)
                        {
                            responseContent = await httpContent.ReadAsStringAsync();
                            if (httpResponse.IsSuccessStatusCode)
                            {
                                var response = JsonConvert.DeserializeObject<dynamic>(responseContent);

                                return response.id;
                            }
                            else
                            {
                                Console.ForegroundColor = ConsoleColor.Red;
                                Console.Write("[MESSAGE BODY] ");
                                Console.ForegroundColor = ConsoleColor.Gray;
                                Console.WriteLine(responseContent);

                                return null;
                            }
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(e);

                return null;
            }
        }
        #endregion

#endif

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