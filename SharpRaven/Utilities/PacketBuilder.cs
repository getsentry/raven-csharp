using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SharpRaven.Utilities {
    public static class PacketBuilder {
        public static string CreateAuthenticationHeader(DSN dsn) {
            string header = String.Empty;
            header += "Sentry sentry_version=4";
            header += ", sentry_client=SharpRaven/1.0";
            header += ", sentry_timestamp=" + (long)(DateTime.UtcNow - new DateTime(1970, 1, 1)).TotalSeconds;
            header += ", sentry_key=" + dsn.PublicKey;
            header += ", sentry_secret=" + dsn.PrivateKey;

            return header;
        }
    }
}
