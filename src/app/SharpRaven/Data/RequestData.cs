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

using Newtonsoft.Json;

namespace SharpRaven.Data
{
    /// <summary>
    /// Contains <see cref="string"/> representations of a <see cref="JsonPacket"/>.
    /// </summary>
    public class RequestData
    {
        private readonly Requester requester;
        private string formatted;
        private string raw;
        private string scrubbed;


        /// <summary>
        /// Initializes a new instance of the <see cref="RequestData"/> class.
        /// </summary>
        /// <param name="requester">The <see cref="Requester"/> in which the request data will be used.</param>
        internal RequestData(Requester requester)
        {
            if (requester == null)
                throw new ArgumentNullException("requester");

            if (requester.Packet == null)
                throw new ArgumentException("Requester.Packet was null", "requester");

            this.requester = requester;
        }


        /// <summary>
        /// Gets the <see cref="JsonPacket"/> converted to a <see cref="System.String"/> before
        /// being scrubbed by <see cref="SharpRaven.Logging.IScrubber"/>.
        /// </summary>
        public string Raw
        {
            get { return this.raw = this.raw ?? this.requester.Packet.ToString(Formatting.None); }
        }

        /// <summary>
        /// Gets the <see cref="JsonPacket"/> converted to a <see cref="System.String"/> after
        /// being scrubbed by <see cref="M:IRavenClient.LogScubber"/>. If <see cref="M:IRavenClient.LogScubber"/>
        /// is null, <see cref="Raw"/> will be returned.
        /// </summary>
        public string Scrubbed
        {
            get
            {
                if (this.requester.Client == null || this.requester.Client.LogScrubber == null)
                    return Raw;

                return this.scrubbed = this.scrubbed ?? this.requester.Client.LogScrubber.Scrub(Raw);
            }
        }


        /// <summary>
        /// Gets a <see cref="System.String"/> representation of the <see cref="RequestData"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String"/> representation of the <see cref="RequestData"/>.
        /// </returns>
        public override string ToString()
        {
            return this.formatted = this.formatted ?? this.requester.Packet.ToString(Formatting.Indented);
        }
    }
}