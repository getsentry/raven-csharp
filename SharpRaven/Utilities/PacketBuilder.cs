using System;

namespace SharpRaven.Utilities
{
    /// <summary>
    /// Utility class for building 
    /// </summary>
    public static class PacketBuilder
    {
        private static string userAgent;


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
            private set { userAgent = value; }
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
            return String.Format("Sentry sentry_version=4"
                                 + ", sentry_client={0}"
                                 + ", sentry_timestamp={1}"
                                 + ", sentry_key={2}"
                                 + ", sentry_secret={3}",
                                 UserAgent,
                                 (long) (DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds,
                                 dsn.PublicKey,
                                 dsn.PrivateKey);
        }
    }
}