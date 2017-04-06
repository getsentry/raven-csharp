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

#if !(net40) && !(net35)

using System;
using System.IO;
using System.IO.Compression;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

using Newtonsoft.Json.Linq;

using SharpRaven.Utilities;

namespace SharpRaven.Data
{
    public partial class Requester
    {
        private readonly HttpClient httpClient;


        /// <summary>
        /// Initializes a new instance of the <see cref="Requester"/> class.
        /// </summary>
        /// <param name="ravenClient">The <see cref="RavenClient"/> to initialize with.</param>
        internal Requester(RavenClient ravenClient)
        {
            if (ravenClient == null)
                throw new ArgumentNullException("ravenClient");

            this.ravenClient = ravenClient;
            this.httpClient = new HttpClient
            {
                Timeout = ravenClient.Timeout
            };

            var auth = PacketBuilder.CreateAuthenticationHeader(ravenClient.CurrentDsn);
            var userAgent = new ProductInfoHeaderValue(PacketBuilder.ProductName, PacketBuilder.ProductVersion);

            this.httpClient.DefaultRequestHeaders.Add("X-Sentry-Auth", auth);
            this.httpClient.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            this.httpClient.DefaultRequestHeaders.UserAgent.Add(userAgent);
        }


        /// <summary>
        /// Disposes the <see cref="Requester"/> and its <see cref="HttpClient"/>.
        /// </summary>
        public void Dispose()
        {
            if (this.httpClient != null)
                this.httpClient.Dispose();
        }


        /// <summary>
        /// Executes the <c>async</c> HTTP request to Sentry.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        public string Request(JsonPacket packet)
        {
            return RequestAsync(packet, CancellationToken.None).GetAwaiter().GetResult();
        }


        /// <summary>
        /// Executes the <c>async</c> HTTP request to Sentry.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonPacket.EventID" /> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        public async Task<string> RequestAsync(JsonPacket packet, CancellationToken ct)
        {
            if (this.data == null)
                throw new InvalidOperationException("Cannot perform a request on an unprepared Requester. Call Prepare() first.");

            using (var request = new HttpRequestMessage(HttpMethod.Post, this.ravenClient.CurrentDsn.SentryUri))
            {
                foreach (var header in request.Headers)
                {
                    this.Data.Headers.Add(header.Key, header.Value);
                }

                request.Content = new StringContent(this.Data.Scrubbed);

                if (Client.Compression)
                    request.Content = new CompressedContent(request.Content, "gzip");

                using (var response = await this.httpClient.SendAsync(request, ct).ConfigureAwait(false))
                {
                    response.EnsureSuccessStatusCode();
                    var content = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    var o = JObject.Parse(content);
                    return o["id"] != null ? o["id"].ToString() : null;
                }
            }
        }


        /// <summary>
        /// Compressed <see cref="HttpContent"/> class.
        /// </summary>
        /// <remarks>
        /// Shamefully snitched from https://github.com/WebApiContrib/WebAPIContrib/blob/master/src/WebApiContrib/Content/CompressedContent.cs.
        /// </remarks>
        private class CompressedContent : HttpContent
        {
            private readonly string encodingType;
            private readonly HttpContent originalContent;


            public CompressedContent(HttpContent content, string encodingType)
            {
                if (content == null)
                    throw new ArgumentNullException("content");

                if (encodingType == null)
                    throw new ArgumentNullException("encodingType");

                this.originalContent = content;
                this.encodingType = encodingType.ToLowerInvariant();

                if (this.encodingType != "gzip" && this.encodingType != "deflate")
                    throw new NotSupportedException(
                        string.Format("Encoding '{0}' is not supported. Only supports gzip or deflate encoding.", this.encodingType));

                foreach (var header in this.originalContent.Headers)
                    Headers.TryAddWithoutValidation(header.Key, header.Value);

                Headers.ContentEncoding.Add(encodingType);
            }


            protected override void Dispose(bool disposing)
            {
                if (disposing)
                    this.originalContent.Dispose();

                base.Dispose(disposing);
            }


            protected override Task SerializeToStreamAsync(Stream stream, TransportContext context)
            {
                Stream compressedStream = null;

                switch (this.encodingType)
                {
                    case "gzip":
                        compressedStream = new GZipStream(stream, CompressionMode.Compress, true);
                        break;
                    case "deflate":
                        compressedStream = new DeflateStream(stream, CompressionMode.Compress, true);
                        break;
                }

                return this.originalContent.CopyToAsync(compressedStream).ContinueWith(task =>
                {
                    if (compressedStream != null)
                        compressedStream.Dispose();
                });
            }


            protected override bool TryComputeLength(out long length)
            {
                length = -1;

                return false;
            }
        }
    }
}

#endif