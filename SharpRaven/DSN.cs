using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRaven {
    public class DSN {
        /// <summary>
        /// Absolute DSN Uri
        /// </summary>
        public Uri URI { get; set; }
        /// <summary>
        /// Sentry URI for sending reports.
        /// </summary>
        public string SentryURI { get; set; }
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
        /// Project identification.
        /// </summary>
        public string Path { get; set; }

        public DSN(string dsn) {
            bool useSSl = dsn.StartsWith("https", StringComparison.InvariantCultureIgnoreCase);
            Uri URI = new Uri(dsn);

            // Set all info
            PrivateKey = GetPrivateKey(URI);
            PublicKey = GetPublicKey(URI);
            Port = GetPort(URI);
            ProjectID = GetProjectID(URI);
            Path = GetPath(URI);

            SentryURI = String.Format(@"{0}://{1}:{2}{3}/api/{4}/store/", 
                useSSl ? "https" : "http",
                URI.DnsSafeHost,
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
        /// Get a path from a DSN uri
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPath(Uri uri)
        {
            int lastSlash = uri.AbsolutePath.LastIndexOf("/");
            return uri.AbsolutePath.Substring(0, lastSlash);
        }

        /// <summary>
        /// Get a public key from a DSN uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPublicKey(Uri uri) {
            return uri.UserInfo.Split(':')[0];
        }

        /// <summary>
        /// Get a private key from a DSN uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetPrivateKey(Uri uri) {
            return uri.UserInfo.Split(':')[1];
        }

        /// <summary>
        /// Get a project ID from a DSN uri.
        /// </summary>
        /// <param name="uri"></param>
        /// <returns></returns>
        public string GetProjectID(Uri uri) {
            int lastSlash = uri.AbsoluteUri.LastIndexOf("/");
            return uri.AbsoluteUri.Substring(lastSlash + 1);
        }
    }
}
