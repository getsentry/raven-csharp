using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Com.Sentry.Json {
    public class ReportPacket {
        /// <summary>
        /// Hexadecimal string representing a uuid4 value.
        /// </summary>
        public string event_id { get; set; }
        /// <summary>
        /// String value representing the project
        /// </summary>
        public string project { get; set; }
        /// <summary>
        /// Function call which was the primary perpetrator of this event.
        /// </summary>
        public string culprit { get; set; }
        /// <summary>
        /// Indicates when the logging record was created (in the Sentry client).
        /// defaults to DateTime.Now
        /// </summary>
        public DateTime timestamp { get; set; }
        /// <summary>
        /// User-readable representation of this event
        /// </summary>
        public string message { get; set; }
        /// <summary>
        /// An arbitrary mapping of additional metadata to store with the event.
        /// </summary>
        public Dictionary<string, object> extra { get; set; }
        /// <summary>
        /// A list of relevant modules and their versions.
        /// </summary>
        public Dictionary<string, double> modules { get; set; }
        /// <summary>
        /// Identifies the host client from which the event was recorded.
        /// </summary>
        public string server_name { get; set; }
        /// <summary>
        /// The record severity.
        /// Defaults to logging.ERROR.
        /// </summary>
        public string level { get; set; }
        /// <summary>
        /// The name of the logger which created the record.
        /// </summary>
        public string logger { get; set; }
        /// <summary>
        /// A map or list of tags for this event.
        /// </summary>
        public Dictionary<string, string> tags { get; set; }
        public Dictionary<string, string> Exception { get; set; }

        public ReportPacket(Exception e) {
            InitializeFields();
        }

        private void InitializeFields() {
            Exception = new Dictionary<string, string>();
            tags = new Dictionary<string, string>();
            modules = new Dictionary<string, double>();
            extra = new Dictionary<string, object>();
        }
    }
}
