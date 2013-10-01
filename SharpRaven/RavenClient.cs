using System;
using System.Collections.Generic;
using System.IO;
using System.Net;

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
        {
            CurrentDsn = new Dsn(dsn);
            Compression = true;
            Logger = "root";
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
        /// <returns></returns>
        public int CaptureException(Exception e)
        {
            return CaptureException(e, null, null);
        }


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        public int CaptureException(Exception e, IDictionary<string, string> tags = null)
        {
            return CaptureException(e, tags, null);
        }


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception" /> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns></returns>
        public int CaptureException(Exception e, IDictionary<string, string> tags = null, object extra = null)
        {
            JsonPacket packet = new JsonPacket(CurrentDsn.ProjectID, e)
            {
                Level = ErrorLevel.Error,
                Tags = tags,
                Extra = extra
            };

            Send(packet, CurrentDsn);

            return 0;
        }


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <returns></returns>
        public int CaptureMessage(string message)
        {
            return CaptureMessage(message, ErrorLevel.Info, null, null);
        }


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured message.</param>
        /// <returns></returns>
        public int CaptureMessage(string message, ErrorLevel level)
        {
            return CaptureMessage(message, level, null, null);
        }


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured message.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        public int CaptureMessage(string message, ErrorLevel level, Dictionary<string, string> tags)
        {
            return CaptureMessage(message, level, tags, null);
        }


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel" /> of the captured message.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns></returns>
        public int CaptureMessage(string message,
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

            Send(packet, CurrentDsn);

            return 0;
        }


        /// <summary>
        /// Sends the specified packet to Sentry.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <returns>
        /// <c>true</c> if the <see cref="Send"/> succeeds, <c>false</c> if it fails.
        /// </returns>
        private bool Send(JsonPacket packet, Dsn dsn)
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
                request.UserAgent = "RavenSharp/1.0";

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
                        // Close streams.
                        sw.Flush();
                        sw.Close();
                    }
                    s.Flush();
                    s.Close();
                }

                using (HttpWebResponse wr = (HttpWebResponse) request.GetResponse())
                {
                    wr.Close();
                }
            }
            catch (WebException e)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(e.Message);

                string messageBody = String.Empty;
                if (e.Response != null)
                {
                    using (StreamReader sw = new StreamReader(e.Response.GetResponseStream()))
                    {
                        messageBody = sw.ReadToEnd();
                    }
                    Console.WriteLine("[MESSAGE BODY] " + messageBody);
                }

                return false;
            }

            return true;
        }

        #region Deprecated methods

        /**
         *  These methods have been deprectaed in favour of the ones
         *  that have the same names as the other sentry clients, this
         *  is purely for the sake of consistency
         */


        [Obsolete("The more common CaptureException method should be used")]
        public int CaptureEvent(Exception e)
        {
            return CaptureException(e);
        }


        [Obsolete("The more common CaptureException method should be used")]
        public int CaptureEvent(Exception e, Dictionary<string, string> tags)
        {
            return CaptureException(e, tags);
        }

        #endregion
    }
}