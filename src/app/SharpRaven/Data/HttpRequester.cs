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

#if NET35
using JObject = Newtonsoft.Json.Linq.JObject;
#endif

namespace SharpRaven.Data
{
    /// <summary>
    /// The class responsible for performing the HTTP request to Sentry.
    /// </summary>
    public partial class HttpRequester : IRequester
    {
        private readonly bool useCompression;
        private readonly TimeSpan timeout;
        private readonly RequestData data;
        private readonly SentryUserFeedback feedback;
        private readonly HttpWebRequest webRequest;


        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequester"/> class.
        /// </summary>
        /// <param name="data">The request data.</param>
        /// <param name="dsn">The DSN used in this request</param>
        /// <param name="timeout">The request timeout.</param>
        /// <param name="useCompression">Whether to use compression or not.</param>
        public HttpRequester(RequestData data, Dsn dsn, TimeSpan timeout, bool useCompression)
        {
            this.data = data ?? throw new ArgumentNullException(nameof(data));
            this.timeout = timeout == default ? TimeSpan.FromSeconds(5) : timeout;
            this.useCompression = useCompression;

			this.webRequest = CreateWebRequest(dsn.SentryUri, dsn);

            if (useCompression)
            {
                this.webRequest.Headers.Add(HttpRequestHeader.ContentEncoding, "gzip");
                this.webRequest.AutomaticDecompression = DecompressionMethods.Deflate;
                this.webRequest.ContentType = "application/octet-stream";
            }

            else
                this.webRequest.ContentType = "application/json; charset=utf-8";

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HttpRequester"/> class.
        /// </summary>
        /// <param name="feedback">The <see cref="SentryUserFeedback"/> to initialize with.</param>
        /// <param name="dsn">The DSN used in this request</param>
        internal HttpRequester(SentryUserFeedback feedback, Dsn dsn)
        {
            if (dsn == null) throw new ArgumentNullException(nameof(dsn));
            this.feedback = feedback ?? throw new ArgumentNullException(nameof(feedback));

            var feedbackUri = new Uri($"{dsn.FeedbackUri}?dsn={dsn.Uri}&{feedback}");

            this.webRequest = CreateWebRequest(feedbackUri, dsn);
            this.webRequest.Referer = dsn.Uri.DnsSafeHost;

            this.webRequest.ContentType = "application/x-www-form-urlencoded";
        }

		internal HttpWebRequest CreateWebRequest(Uri uri, Dsn dsn)
		{
			var request = (HttpWebRequest)System.Net.WebRequest.Create(uri);
			request.Timeout = (int)this.timeout.TotalMilliseconds;
			request.ReadWriteTimeout = (int)this.timeout.TotalMilliseconds;
			request.Method = "POST";
			request.Accept = "application/json";
			request.Headers.Add("X-Sentry-Auth", PacketBuilder.CreateAuthenticationHeader(dsn));
			request.UserAgent = PacketBuilder.UserAgent;
			return request;
		}

        /// <summary>
        /// 
        /// </summary>
        public RequestData Data
        {
            get { return this.data; }
        }

        /// <summary>
        /// Gets the <see cref="HttpWebRequest"/> instance being used to perform the HTTP request to Sentry.
        /// </summary>
        public HttpWebRequest WebRequest
        {
            get { return this.webRequest; }
        }

        /// <summary>
        /// Uses the event identifier on the JsonPacket if there is one.
        /// </summary>
        /// <param name="eventId">The event identifier.</param>
        public void UseEventId(string eventId)
        {
            if (Data?.JsonPacket != null)
            {
                Data.JsonPacket.EventID = eventId;
            }
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
                if (this.useCompression)
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
                        #if (NET35)
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

        /// <summary>
        /// Sends the user feedback to sentry
        /// </summary>
        /// <returns>An empty string if succesful, otherwise the server response</returns>
        public string SendFeedback()
        {
            using (var s = this.webRequest.GetRequestStream())
            {
                using (var sw = new StreamWriter(s))
                {
                    sw.Write(feedback.ToString());
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
                        var response = sr.ReadToEnd();
                        return response;
                    }
                }
            }
        }
    }
}