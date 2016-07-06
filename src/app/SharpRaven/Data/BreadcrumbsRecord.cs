using System;

namespace SharpRaven.Data {
    /// <summary>
    /// BreadcrumbsRecord trail.
    /// </summary>
    public class BreadcrumbsRecord {
        private readonly BreadcrumbsType? breadcrumbsType;
        private readonly DateTime timestamp;

        public BreadcrumbsRecord() {
            Category = "log";
            timestamp = DateTime.UtcNow;
        }


        public BreadcrumbsRecord(BreadcrumbsType type) {
            breadcrumbsType = type;
        }


        public BreadcrumbsType? Type {
            get { return breadcrumbsType; }
        }

        public string Category { get; set; }

        public DateTime Timestamp { get { return timestamp; } }

        public string Message { get; set; }
    }
}
