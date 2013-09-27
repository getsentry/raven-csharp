using System;
using System.Collections.Generic;
using System.Text;

namespace SharpRaven {
    /// <summary>
    /// The Data Source Name of a given project in Sentry.
    /// </summary>
    public class Dsn {
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
        /// Initializes a new instance of the <see cref="Dsn"/> class.
        /// </summary>
        /// <param name="dsn">The Data Source Name.</param>
        public Dsn(string dsn) {
            bool useSSl = dsn.StartsWith("https", StringComparison.InvariantCultureIgnoreCase);
            Uri uri = new Uri(dsn);

            // Set all info
            PrivateKey = GetPrivateKey(uri);
            PublicKey = GetPublicKey(uri);
            Port = GetPort(uri);
            ProjectID = GetProjectID(uri);
            Path = GetPath(uri);

            this.SentryUri = String.Format(@"{0}://{1}:{2}{3}/api/{4}/store/", 
                useSSl ? "https" : "http",
                uri.DnsSafeHost,
                Port,
                Path,
                ProjectID);
        }

        /// <summary>
        /// Get port from Uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public int GetPort(Uri uri) {
            return uri.Port;
        }

        /// <summary>
        /// Get a path from a Dsn uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPath(Uri uri)
        {
            int lastSlash = uri.AbsolutePath.LastIndexOf("/");
            return uri.AbsolutePath.Substring(0, lastSlash);
        }

        /// <summary>
        /// Get a public key from a Dsn uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPublicKey(Uri uri) {
            return uri.UserInfo.Split(':')[0];
        }

        /// <summary>
        /// Get a private key from a Dsn uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPrivateKey(Uri uri) {
            return uri.UserInfo.Split(':')[1];
        }

        /// <summary>
        /// Get a project ID from a Dsn uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetProjectID(Uri uri) {
            int lastSlash = uri.AbsoluteUri.LastIndexOf("/");
            return uri.AbsoluteUri.Substring(lastSlash + 1);
        }
    }
}
