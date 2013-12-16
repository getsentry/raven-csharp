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

namespace SharpRaven
{
    /// <summary>
    /// The Data Source Name of a given project in Sentry.
    /// </summary>
    public class Dsn
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Dsn"/> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name.</param>
        public Dsn(string dsn)
        {
            bool useSSl = dsn.StartsWith("https", StringComparison.InvariantCultureIgnoreCase);
            Uri uri = new Uri(dsn);

            // Set all info
            PrivateKey = GetPrivateKey(uri);
            PublicKey = GetPublicKey(uri);
            Port = GetPort(uri);
            ProjectID = GetProjectID(uri);
            Path = GetPath(uri);

            SentryUri = String.Format(@"{0}://{1}:{2}{3}/api/{4}/store/",
                                      useSSl ? "https" : "http",
                                      uri.DnsSafeHost,
                                      Port,
                                      Path,
                                      ProjectID);
        }


        /// <summary>
        /// Absolute Dsn Uri
        /// </summary>
        public Uri Uri { get; set; }

        /// <summary>
        /// Sentry Uri for sending reports.
        /// </summary>
        public string SentryUri { get; set; }

        /// <summary>
        /// Project public key.
        /// </summary>
        public string PublicKey { get; set; }

        /// <summary>
        /// Project private key.
        /// </summary>
        public string PrivateKey { get; set; }

        /// <summary>
        /// Project identification.
        /// </summary>
        public string ProjectID { get; set; }

        /// <summary>
        /// The sentry server port.
        /// </summary>
        public int Port { get; set; }

        /// <summary>
        /// Sentry path.
        /// </summary>
        public string Path { get; set; }


        /// <summary>
        /// Get port from Uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public int GetPort(Uri uri)
        {
            return uri.Port;
        }


        /// <summary>
        /// Get a path from a Dsn uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPath(Uri uri)
        {
            int lastSlash = uri.AbsolutePath.LastIndexOf("/", StringComparison.Ordinal);
            return uri.AbsolutePath.Substring(0, lastSlash);
        }


        /// <summary>
        /// Get a public key from a Dsn uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPublicKey(Uri uri)
        {
            return uri.UserInfo.Split(':')[0];
        }


        /// <summary>
        /// Get a private key from a Dsn uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPrivateKey(Uri uri)
        {
            return uri.UserInfo.Split(':')[1];
        }


        /// <summary>
        /// Get a project ID from a Dsn uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetProjectID(Uri uri)
        {
            int lastSlash = uri.AbsoluteUri.LastIndexOf("/", StringComparison.Ordinal);
            return uri.AbsoluteUri.Substring(lastSlash + 1);
        }
    }
}