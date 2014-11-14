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

using System.Threading;

using Nancy;

using SharpRaven.Data;

namespace SharpRaven.Nancy
{
    /// <summary>
    /// The Raven Client, responsible for capturing exceptions and sending them to Sentry.
    /// </summary>
    public class RavenClient : SharpRaven.RavenClient
    {
        private NancyContext httpContext;


        /// <summary>
        /// Initializes a new instance of the <see cref="RavenClient" /> class. Sentry
        /// Data Source Name will be read from sharpRaven section in your app.config or
        /// web.config.
        /// </summary>
        /// <param name="httpContext">The optional <see cref="NancyContext" /> that will be used to fill <see cref="SentryRequest" /> that will be sent to Sentry.</param>
        /// <param name="jsonPacketFactory">The optional factory that will be used to create the <see cref="JsonPacket" /> that will be sent to Sentry.</param>
        public RavenClient(NancyContext httpContext = null, IJsonPacketFactory jsonPacketFactory = null)
            : base(new Dsn(Configuration.Settings.Dsn.Value), jsonPacketFactory)
        {
            this.httpContext = httpContext;
        }


        /// <summary>
        /// Sends the specified packet to Sentry.
        /// </summary>
        /// <param name="packet">The packet to send.</param>
        /// <param name="dsn">The Data Source Name in Sentry.</param>
        /// <returns>
        /// The <see cref="JsonPacket.EventID"/> of the successfully captured JSON packet, or <c>null</c> if it fails.
        /// </returns>
        protected override string Send(JsonPacket packet, Dsn dsn)
        {
            // looking for NancyContext
            this.httpContext = this.httpContext ?? Thread.GetData(
                Thread.GetNamedDataSlot(Configuration.Settings.NancyContextDataSlot)) as NancyContext;

            // get SentryRequest
            ISentryRequest sentryRequest = Data.SentryRequest.GetRequest(this.httpContext);

            // patch JsonPacket.Request with data on NancyContext
            packet.Request = sentryRequest;

            // patch JsonPacket.User with data on NancyContext
            packet.User = sentryRequest.GetUser();

            return base.Send(packet, dsn);
        }
    }
}