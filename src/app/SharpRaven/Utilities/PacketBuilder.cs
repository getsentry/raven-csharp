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