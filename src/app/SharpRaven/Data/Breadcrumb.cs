using System;
using System.Collections.Generic;

using Newtonsoft.Json;

using SharpRaven.Serialization;

namespace SharpRaven.Data {
    /// <summary>
    /// Breadcrumb trail.
    /// </summary>
    public class Breadcrumb {
        private readonly DateTime timestamp;

        public Breadcrumb(string category) 
        {
            Category = category;
            this.timestamp = DateTime.UtcNow;
        }


        public Breadcrumb(string category, BreadcrumbType type):this(category)
        {
            Type = type;
        }

        [JsonProperty(PropertyName = "type", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(LowerInvariantStringEnumConverter))]
        public BreadcrumbType? Type { get; set; }

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
        [JsonConverter(typeof(LowerInvariantStringEnumConverter))]
        public BreadcrumbLevel? Level { get; set; }
    }
}
