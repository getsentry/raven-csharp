using System;
using SharpRaven.Data;
using System.Net;
using System.IO;
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

        public int CaptureEvent(Exception e) {
            JsonPacket packet = new JsonPacket(CurrentDSN.ProjectID, e);
            packet.Logger = Logger;
            Send(packet, CurrentDSN);
            return 0;
        }

        public int CaptureEvent(Exception e, string[] tags) {
            return 0;
        }

        public bool Send(JsonPacket jp, DSN dsn) {
            try {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(dsn.SentryURI);
                request.Method = "POST";
                request.Accept = "application/json";
                request.ContentType = "application/json; charset=utf-8";
                request.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3;
                request.UserAgent = "RavenSharp/1.0";

                Console.WriteLine("Header: " + PacketBuilder.CreateAuthenticationHeader(dsn));
                Console.WriteLine("Packet: " + jp.Serialize());

                // Write the messagebody.
                using (Stream s = request.GetRequestStream()) {
                    using (StreamWriter sw = new StreamWriter(s)) {
                        // Compress and encode.
                        //string data = Utilities.GzipUtil.CompressEncode(jp.Serialize());
                        //Console.WriteLine("Writing: " + data);
                        // Write to the JSON script when ready.
                        string data = jp.Serialize();
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

                HttpWebResponse wr = (HttpWebResponse)request.GetResponse();
            } catch (WebException e) {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.Write("[ERROR] ");
                Console.ForegroundColor = ConsoleColor.Gray;
                Console.WriteLine(e.Message);

                string messageBody = String.Empty;
                using (StreamReader sw = new StreamReader(e.Response.GetResponseStream())) {
                    messageBody = sw.ReadToEnd();
                }
                Console.WriteLine("[MESSAGE BODY] " + messageBody);
                
                return false;
            }

            return true;
        }
    }
}
