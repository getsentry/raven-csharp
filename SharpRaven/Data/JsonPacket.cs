using System;
using System.Collections.Generic;

using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using SharpRaven.Serialization;

namespace SharpRaven.Data
{
    /// <summary>
    /// The JSON packet sent to Sentry.
    /// </summary>
    public class JsonPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPacket"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        public JsonPacket(string project)
        {
            // Get assemblies.
            /*Modules = new List<Module>();
            foreach (System.Reflection.Module m in Utilities.SystemUtil.GetModules()) {
                Modules.Add(new Module() {
                    Name = m.ScopeName,
                    Version = m.ModuleVersionId.ToString()
                });
            }*/
            // The current hostname
            ServerName = Environment.MachineName;
            // Create timestamp
            TimeStamp = DateTime.UtcNow;
            // Default logger.
            Logger = "root";
            // Default error level.
            Level = ErrorLevel.Error;
            // Create a guid.
            EventID = Guid.NewGuid().ToString().Replace("-", String.Empty);
            // Project
            Project = "default";
            // Platform
            Platform = "csharp";
            Project = project;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPacket"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="e">The <see cref="Exception"/>.</param>
        public JsonPacket(string project, Exception e)
            : this(project)
        {
            Message = e.Message;

            if (e.TargetSite != null)
            {
// ReSharper disable ConditionIsAlwaysTrueOrFalse => not for dynamic types.
                Culprit = String.Format("{0} in {1}",
                                        ((e.TargetSite.ReflectedType == null)
                                             ? "<dynamic type>"
                                             : e.TargetSite.ReflectedType.FullName),
                                        e.TargetSite.Name);
// ReSharper restore ConditionIsAlwaysTrueOrFalse
            }

            Project = project;
            ServerName = Environment.MachineName;
            Level = ErrorLevel.Error;

            Exceptions = new List<SentryException>();

            for (Exception currentException = e;
                 currentException != null;
                 currentException = currentException.InnerException)
            {
                SentryException sentryException = new SentryException(currentException)
                {
                    Module = currentException.Source,
                    Type = currentException.GetType().Name,
                    Value = currentException.Message
                };
                Exceptions.Add(sentryException);
            }
        }


        /// <summary>
        /// An arbitrary mapping of additional metadata to store with the event.
        /// </summary>
        [JsonProperty(PropertyName = "extra", NullValueHandling = NullValueHandling.Ignore)]
        public object Extra { get; set; }

        /// <summary>
        /// A map or list of tags for this event.
        /// </summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Tags { get; set; }


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
        [JsonProperty(PropertyName = "level", NullValueHandling = NullValueHandling.Ignore, Required = Required.Always)]
        [JsonConverter(typeof(ErrorLevelConverter))]
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

        [JsonProperty(PropertyName = "exception", NullValueHandling = NullValueHandling.Ignore)]
        public List<SentryException> Exceptions { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}