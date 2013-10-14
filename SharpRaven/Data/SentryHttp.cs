using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace SharpRaven.Data
{
    public class SentryHttp
    {
        private readonly dynamic httpContext;


        public SentryHttp()
        {
            // NOTE: We're using dynamic to not require a reference to System.Web.
            this.httpContext = GetHttpContext();

            if (!HasHttpContext)
                return;

            Url = this.httpContext.Request.RawUrl;
            Method = this.httpContext.Request.HttpMethod;
        }


        [JsonIgnore]
        public bool HasHttpContext
        {
            get { return this.httpContext != null; }
        }


        [JsonProperty(PropertyName = "url", NullValueHandling = NullValueHandling.Ignore)]
        public string Url { get; set; }

        [JsonProperty(PropertyName = "method", NullValueHandling = NullValueHandling.Ignore)]
        public string Method { get; set; }

        [JsonProperty(PropertyName = "data", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Data { get; set; }

        [JsonProperty(PropertyName = "query_string", NullValueHandling = NullValueHandling.Ignore)]
        public string QueryString { get; set; }

        [JsonProperty(PropertyName = "cookies", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Cookies { get; set; }

        [JsonProperty(PropertyName = "headers", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Headers { get; set; }

        [JsonProperty(PropertyName = "env", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Environment { get; set; }


        private static dynamic GetHttpContext()
        {
            var systemWeb = AppDomain.CurrentDomain
                                     .GetAssemblies()
                                     .FirstOrDefault(assembly => assembly.FullName.StartsWith("System.Web"));

            if (systemWeb == null)
                return null;

            var httpContextType = systemWeb.GetExportedTypes()
                                           .FirstOrDefault(type => type.Name == "HttpContext");

            if (httpContextType == null)
                return null;

            var currentHttpContextProperty = httpContextType.GetProperty("Current",
                                                                         BindingFlags.Static | BindingFlags.Public);

            if (currentHttpContextProperty == null)
                return null;

            return currentHttpContextProperty.GetValue(null, null);
        }
    }
}