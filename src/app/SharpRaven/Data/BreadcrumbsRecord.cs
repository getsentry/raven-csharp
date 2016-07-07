using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using SharpRaven.Serialization;

namespace SharpRaven.Data {
    /// <summary>
    /// BreadcrumbsRecord trail.
    /// </summary>
    public class BreadcrumbsRecord {
        private readonly DateTime timestamp;

        public BreadcrumbsRecord() 
        {
            Category = "log";
            this.timestamp = DateTime.UtcNow;
        }


        public BreadcrumbsRecord(BreadcrumbsType type):this()
        {
            Type = type;
        }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(LowerInvariantConverter))]
        public BreadcrumbsType? Type { get; set; }

        [JsonProperty(PropertyName = "category", NullValueHandling = NullValueHandling.Ignore)]
        public string Category { get; set; }

        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime Timestamp
        {
            get { return this.timestamp; }
        }

        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Data { get; set; }

        [JsonProperty(PropertyName = "level", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(LowerInvariantConverter))]
        public BreadcrumbsLevel? Level { get; set; }
    }
}
