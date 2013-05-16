using System;
using SharpRaven.Data;
using System.Net;
using System.IO;
using System.Collections.Generic;
using SharpRaven.Utilities;
using SharpRaven.Logging;

namespace SharpRaven {
    public class RavenClient {

        /// <summary>
        /// The DSN currently being used to log exceptions.
        /// </summary>
        public DSN CurrentDSN { get; set; }

        /// <summary>
        /// Interface for providing a 'log scrubber' that removes 
        /// sensitive information from exceptions sent to sentry.
        /// </summary>
        public IScrubber LogScrubber { get; set; }

        /// <summary>
        /// Enable Gzip Compression?
        /// Defaults to true.
        /// </summary>
        public bool Compression { get; set; }

        /// <summary>
        /// Logger. Default is "root"
        /// </summary>
        public string Logger { get; set; }

        public RavenClient(string dsn) {
            CurrentDSN = new DSN(dsn);
            Compression = true;
            Logger = "root";
        }

        public RavenClient(DSN dsn) {
            CurrentDSN = dsn;
            Compression = true;
            Logger = "root";
        }

        public int CaptureException(Exception e)
        {
            return CaptureException(e, null, null);
        }

        public int CaptureException(Exception e, Dictionary<string, string> tags = null)
        {
            return CaptureException(e, tags, null);
        }

        public int CaptureException(Exception e, Dictionary<string, string> tags = null, object extra = null)
        {
            JsonPacket packet = new JsonPacket(CurrentDSN.ProjectID, e);
            packet.Level = ErrorLevel.error;
            packet.Tags = tags;
            packet.Extra = extra;

            Send(packet, CurrentDSN);

            return 0;
        }

        public int CaptureMessage(string message)
        {
            return CaptureMessage(message, ErrorLevel.info, null, null);
        }

        public int CaptureMessage(string message, ErrorLevel level)
        {
            return CaptureMessage(message, level, null, null);
        }

        public int CaptureMessage(string message, ErrorLevel level, Dictionary<string, string> tags)
        {
            return CaptureMessage(message, level, tags, null);
        }

        public int CaptureMessage(string message, ErrorLevel level = ErrorLevel.info, Dictionary<string, string> tags = null, object extra = null)
        {
            JsonPacket packet = new JsonPacket(CurrentDSN.ProjectID);
            packet.Message = message;
            packet.Level = level;
            packet.Tags = tags;
            packet.Extra = extra;

            Send(packet, CurrentDSN);

            return 0;
        }

        public bool Send(JsonPacket packet, DSN dsn) {
            packet.Logger = Logger;

            try {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(dsn.SentryURI);
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                request.UserAgent = "RavenSharp/1.0";

                Console.WriteLine("Header: " + PacketBuilder.CreateAuthenticationHeader(dsn));
                Console.WriteLine("Packet: " + packet.Serialize());

                // Write the messagebody.
                using (Stream s = request.GetRequestStream()) {
                    using (StreamWriter sw = new StreamWriter(s)) {
                        // Compress and encode.
                        //string data = Utilities.GzipUtil.CompressEncode(packet.Serialize());
                        //Console.WriteLine("Writing: " + data);
                        // Write to the JSON script when ready.
                        string data = packet.Serialize();
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

                using (HttpWebResponse wr = (HttpWebResponse)request.GetResponse())
                {
                    wr.Close();
                }
            } catch (WebException e) {
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
            return this.CaptureException(e);
        }

        [Obsolete("The more common CaptureException method should be used")]
        public int CaptureEvent(Exception e, Dictionary<string, string> tags)
        {
            return this.CaptureException(e, tags);
        }

        #endregion
    }
}
