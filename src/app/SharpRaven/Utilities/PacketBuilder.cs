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

namespace SharpRaven.Utilities
{
    /// <summary>
    /// Utility class for building 
    /// </summary>
    public static class PacketBuilder
    {
        private const int SentryVersion = 4;
        private static readonly string userAgent;


        /// <summary>
        /// Initializes the <see cref="PacketBuilder"/> class.
        /// </summary>
        static PacketBuilder()
        {
            var assemblyName = typeof(PacketBuilder).Assembly.GetName();
            var name = assemblyName.Name;
            var version = assemblyName.Version;
            userAgent = String.Format("{0}/{1}", name, version);
        }


        /// <summary>
        /// Gets the user agent string for Sentry.
        /// </summary>
        /// <value>
        /// The user agent string for Sentry.
        /// </value>
        public static string UserAgent
        {
            get { return userAgent; }
        }


        /// <summary>
        /// Creates the authentication header base on the provided <see cref="Dsn"/>.
        /// </summary>
        /// <param name="dsn">The DSN.</param>
        /// <returns>
        /// The authentication header.
        /// </returns>
        public static string CreateAuthenticationHeader(Dsn dsn)
        {
            return String.Format("Sentry sentry_version={0}"
                                 + ", sentry_client={1}"
                                 + ", sentry_timestamp={2}"
                                 + ", sentry_key={3}"
                                 + ", sentry_secret={4}",
                                 SentryVersion,
                                 UserAgent,
                                 (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                                 dsn.PublicKey,
                                 dsn.PrivateKey);
        }
    }
}