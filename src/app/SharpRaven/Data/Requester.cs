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
using System.IO;
using System.Net;

using Newtonsoft.Json;

using SharpRaven.Utilities;

#if net35
using JObject = Newtonsoft.Json.Linq.JObject;
#endif

namespace SharpRaven.Data
{
    /// <summary>
    /// The class responsible for performing the HTTP request to Sentry.
    /// </summary>
    public partial class Requester
    {
        private readonly RequestData data;
        private readonly JsonPacket packet;
        private readonly RavenClient ravenClient;
        private readonly HttpWebRequest webRequest;


        /// <summary>
        /// Initializes a new instance of the <see cref="Requester"/> class.
        /// </summary>
        /// <param name="packet">The <see cref="JsonPacket"/> to initialize with.</param>
        /// <param name="ravenClient">The <see cref="RavenClient"/> to initialize with.</param>
        internal Requester(JsonPacket packet, RavenClient ravenClient)
        {
            if (packet == null)
                throw new ArgumentNullException("packet");

            if (ravenClient == null)
                throw new ArgumentNullException("ravenClient");

            this.ravenClient = ravenClient;
            this.packet = ravenClient.PreparePacket(packet);
            this.data = new RequestData(this);

            this.webRequest = (HttpWebRequest)System.Net.WebRequest.Create(ravenClient.CurrentDsn.SentryUri);
            this.webRequest.Timeout = (int)ravenClient.Timeout.TotalMilliseconds;
            this.webRequest.ReadWriteTimeout = (int)ravenClient.Timeout.TotalMilliseconds;
            this.webRequest.Method = "POST";
            this.webRequest.Accept = "application/json";
            this.webRequest.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(ravenClient.CurrentDsn));
            this.webRequest.UserAgent = PacketBuilder.UserAgent;

            if (ravenClient.Compression)
            {
                this.webRequest.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");
                this.webRequest.AutomaticDecompression = DecompressionMethods.Deflate;
                this.webRequest.ContentType = "application/octet-stream";
            }
            else
                this.webRequest.ContentType = "application/json; charset=utf-8";
        }


        /// <summary>
        /// Gets the <see cref="IRavenClient"/>.
        /// </summary> 
        public IRavenClient Client
        {
            get { return this.ravenClient; }
        }

        /// <summary>
        /// 
        /// </summary>
        public RequestData Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Gets the <see cref="JsonPacket"/> being sent to Sentry.
        /// </summary>
        public JsonPacket Packet
        {
            get { return this.packet; }
        }

        /// <summary>
        /// Gets the <see cref="HttpWebRequest"/> instance being used to perform the HTTP request to Sentry.
        /// </summary>
        public HttpWebRequest WebRequest
        {
            get { return this.webRequest; }
        }


        /// <summary>
        /// Executes the HTTP request to Sentry.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        public string Request()
        {
            using (var s = this.webRequest.GetRequestStream())
            {
                if (this.ravenClient.Compression)
                    GzipUtil.Write(this.data.Scrubbed, s);
                else
                {
                    using (var sw = new StreamWriter(s))
                    {
                        sw.Write(this.data.Scrubbed);
                    }
                }
            }

            using (var wr = (HttpWebResponse)this.webRequest.GetResponse())
            {
                using (var responseStream = wr.GetResponseStream())
                {
                    if (responseStream == null)
                        return null;

                    using (var sr = new StreamReader(responseStream))
                    {
                        var content = sr.ReadToEnd();
#if (net35)
                        var response = JObject.Parse(content);
                        return response["id"] != null ? response["id"].ToString() : null;
#else
                        var response = JsonConvert.DeserializeObject<dynamic>(content);
                        return response.id;
#endif
                    }
                }
            }
        }
    }
}