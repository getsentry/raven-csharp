using System;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace SharpRaven.Data {
    public class JsonPacket {
        /// <summary>
        /// Hexadecimal string representing a uuid4 value.
        /// </summary>
        [JsonProperty(PropertyName = "event_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EventID { get; set; }

        /// <summary>
        /// String value representing the project
        /// </summary>
        [JsonProperty(PropertyName = "project", NullValueHandling = NullValueHandling.Ignore)]
        public string Project { get; set; }

        /// <summary>
        /// Function call which was the primary perpetrator of this event.
        /// A map or list of tags for this event.
        /// </summary>
        [JsonProperty(PropertyName = "culprit", NullValueHandling = NullValueHandling.Ignore)]
        public string Culprit { get; set; }

        /// <summary>
        /// The record severity.
        /// Defaults to error.
        /// </summary>
        [JsonProperty(PropertyName = "level", NullValueHandling = NullValueHandling.Ignore)]
        [JsonConverter(typeof(Newtonsoft.Json.Converters.StringEnumConverter))]
        public ErrorLevel Level { get; set; }

        /// <summary>
        /// Indicates when the logging record was created (in the Sentry client).
        /// Defaults to DateTime.UtcNow()
        /// </summary>
        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// The name of the logger which created the record.
        /// If missing, defaults to the string root.
        /// 
        /// Ex: "my.logger.name"
        /// </summary>
        [JsonProperty(PropertyName = "logger", NullValueHandling = NullValueHandling.Ignore)]
        public string Logger { get; set; }

        /// <summary>
        /// A string representing the platform the client is submitting from. 
        /// This will be used by the Sentry interface to customize various components in the interface.
        /// </summary>
        [JsonProperty(PropertyName = "platform", NullValueHandling = NullValueHandling.Ignore)]
        public string Platform { get; set; }

        /// <summary>
        /// User-readable representation of this event
        /// </summary>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// A map or list of tags for this event.
        /// </summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public string[][] Tags;

        /// <summary>
        /// An arbitrary mapping of additional metadata to store with the event.
        /// </summary>
        [JsonProperty(PropertyName = "extra", NullValueHandling = NullValueHandling.Ignore)]
        public object Extra;

        /// <summary>
        /// Identifies the host client from which the event was recorded.
        /// </summary>
        [JsonProperty(PropertyName = "server_name", NullValueHandling = NullValueHandling.Ignore)]
        public string ServerName { get; set; }

        /// <summary>
        /// A list of relevant modules (libraries) and their versions.
        /// 
        /// Automated to report all modules currently loaded in project.
        /// </summary>
        [JsonProperty(PropertyName = "modules", NullValueHandling = NullValueHandling.Ignore)]
        public List<Module> Modules { get; set; }

        [JsonProperty(PropertyName="sentry.interfaces.Exception", NullValueHandling=NullValueHandling.Ignore)]
        public SentryException Exception { get; set; }

        [JsonProperty(PropertyName = "sentry.interfaces.Stacktrace", NullValueHandling = NullValueHandling.Ignore)]
        public SentryStacktrace StackTrace { get; set; }

        public JsonPacket(string project) {
            Initialize();
            Project = project;
        }

        public JsonPacket(string project, Exception e) {
            Initialize();
            Message = e.Message;

			if (e.TargetSite != null)
			{
                Culprit = String.Format("{0} in {1}", e.TargetSite.ReflectedType.FullName, e.TargetSite.Name);
			}

            Project = project;
            ServerName = System.Environment.MachineName;
            Level = ErrorLevel.error;

            Exception = new SentryException(e);
            Exception.Module = e.Source;
            Exception.Type = e.GetType().Name;
            Exception.Value = e.Message;

            StackTrace = new SentryStacktrace(e);
        }

        private void Initialize() {
            // Get assemblies.
            /*Modules = new List<Module>();
            foreach (System.Reflection.Module m in Utilities.SystemUtil.GetModules()) {
                Modules.Add(new Module() {
                    Name = m.ScopeName,
                    Version = m.ModuleVersionId.ToString()
                });
            }*/
            // The current hostname
            ServerName = System.Environment.MachineName;
            // Create timestamp
            TimeStamp = DateTime.UtcNow;
            // Default logger.
            Logger = "root";
            // Default error level.
            Level = ErrorLevel.error;
            // Create a guid.
            EventID = Guid.NewGuid().ToString().Replace("-", String.Empty);
            // Project
            Project = "default";
        }

        /// <summary>
        /// Turn into a JSON string.
        /// </summary>
        /// <returns>json string</returns>
        public string Serialize() {
            return JsonConvert.SerializeObject(this);

            //return @"{""project"": ""SharpRaven"",""event_id"": ""fc6d8c0c43fc4630ad850ee518f1b9d0"",""culprit"": ""my.module.function_name"",""timestamp"": ""2012-11-11T17:41:36"",""message"": ""SyntaxError: Wattttt!"",""sentry.interfaces.Exception"": {""type"": ""SyntaxError"",""value"": ""Wattttt!"",""module"": ""__builtins__""}""}";
        }

    }

    public class Module {
        public string Name;
        public string Version;
    }
}
