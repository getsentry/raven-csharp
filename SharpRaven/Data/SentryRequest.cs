using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;

using Newtonsoft.Json;

namespace SharpRaven.Data
{
    public class SentryRequest
    {
        private readonly dynamic httpContext;


        public SentryRequest()
        {
            // NOTE: We're using dynamic to not require a reference to System.Web.
            this.httpContext = GetHttpContext();

            if (!HasHttpContext)
                return;

            Url = this.httpContext.Request.RawUrl;
            Method = this.httpContext.Request.HttpMethod;
            Environment = Convert(x => x.Request.ServerVariables);
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


        private IDictionary<string, string> Convert(Func<dynamic, NameValueCollection> collectionGetter)
        {
            if (this.httpContext == null)
                return null;

            IDictionary<string, string> dictionary = new Dictionary<string, string>();

            try
            {
                NameValueCollection collection = collectionGetter.Invoke(this.httpContext);
                var keys = collection.AllKeys.ToArray();

                foreach (var key in keys)
                {
                    // NOTE: Ignore these keys as they just add duplicate information. [asbjornu]
                    if (key == "ALL_HTTP" || key == "ALL_RAW")
                        continue;

                    var value = collection[key];
                    dictionary.Add(key, value);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }

            return dictionary;
        }


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