using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Sentry {
    public class SentryClient {

        #region Properties
        /// <summary>
        /// The full DSN link.
        /// </summary>
        public string AbsoluteDSN { get; set; }
        /// <summary>
        /// Full DSN uri link.
        /// </summary>
        public Uri AbsoluteDSNUri { get; set; }
        /// <summary>
        /// Sentry public key
        /// </summary>
        public string PublicKey { get; set; }
        /// <summary>
        /// sentry private key
        /// </summary>
        public string PrivateKey { get; set; }
        /// <summary>
        /// sentry project id
        /// </summary>
        public string ProjectID { get; set; }

        public Protocol ClientProtocol { get; set; }
        #endregion

        #region Singleton Initializer
        public static SentryClient Instance {
            get {
                if(instance == null)
                    instance = new SentryClient();
                return instance;
            } 
        }
        public static SentryClient instance;
        #endregion

        #region Constructors
        private SentryClient() {

        }
        #endregion
    }
}
