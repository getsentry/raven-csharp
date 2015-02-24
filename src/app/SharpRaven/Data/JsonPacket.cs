#region License

// Copyright (c) 2014 The Sentry Team and individual contributors.
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
// 
//     1. Redistributions of source code must retain the above copyright notice, this list of
//        conditions and the following disclaimer.
// 
//     2. Redistributions in binary form must reproduce the above copyright notice, this list of
//        conditions and the following disclaimer in the documentation and/or other materials
//        provided with the distribution.
// 
//     3. Neither the name of the Sentry nor the names of its contributors may be used to
//        endorse or promote products derived from this software without specific prior written
//        permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

using System;
using System.Collections.Generic;
using System.Reflection;

using Newtonsoft.Json;

using SharpRaven.Serialization;
using SharpRaven.Utilities;

namespace SharpRaven.Data
{
    /// <summary>
    /// Represents the JSON packet that is transmitted to Sentry.
    /// </summary>
    public class JsonPacket
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPacket"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        /// <param name="exception">The <see cref="Exception"/>.</param>
        public JsonPacket(string project, Exception exception)
            : this(project)
        {
            if (exception == null)
                throw new ArgumentNullException("exception");

            Message = exception.Message;

            if (exception.TargetSite != null)
            {
                // ReSharper disable ConditionIsAlwaysTrueOrFalse => not for dynamic types.
                Culprit = String.Format("{0} in {1}",
                                        ((exception.TargetSite.ReflectedType == null)
                                             ? "<dynamic type>"
                                             : exception.TargetSite.ReflectedType.FullName),
                                        exception.TargetSite.Name);
                // ReSharper restore ConditionIsAlwaysTrueOrFalse
            }

            Exceptions = new List<SentryException>();

            for (Exception currentException = exception;
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

            // ReflectionTypeLoadException doesn't contain much useful info in itself, and needs special handling
            ReflectionTypeLoadException reflectionTypeLoadException = exception as ReflectionTypeLoadException;
            if (reflectionTypeLoadException != null)
            {
                foreach (Exception loaderException in reflectionTypeLoadException.LoaderExceptions)
                {
                    SentryException sentryException = new SentryException(loaderException);

                    Exceptions.Add(sentryException);
                }
            }
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="JsonPacket"/> class.
        /// </summary>
        /// <param name="project">The project.</param>
        public JsonPacket(string project)
            : this()
        {

            if (project == null)
                throw new ArgumentNullException("project");
            
            Project = project;
        }


        /// <summary>
        /// Prevents a default instance of the <see cref="JsonPacket"/> class from being created.
        /// </summary>
        private JsonPacket()
        {
            // Get assemblies.
            Modules = SystemUtil.GetModules();

            // The current hostname
            ServerName = Environment.MachineName;

            // Create timestamp
            TimeStamp = DateTime.UtcNow;

            // Default logger.
            Logger = "root";

            // Default error level.
            Level = ErrorLevel.Error;

            // Create a guid.
            EventID = Guid.NewGuid().ToString("n");

            // Project
            Project = "default";

            // Platform
            Platform = "csharp";
        }


        /// <summary>
        /// Function call which was the primary perpetrator of this event.
        /// A map or list of tags for this event.
        /// </summary>
        [JsonProperty(PropertyName = "culprit", NullValueHandling = NullValueHandling.Ignore)]
        public string Culprit { get; set; }

        /// <summary>
        /// Hexadecimal string representing a uuid4 value.
        /// </summary>
        [JsonProperty(PropertyName = "event_id", NullValueHandling = NullValueHandling.Ignore)]
        public string EventID { get; set; }

        /// <summary>
        /// Gets or sets the exceptions.
        /// </summary>
        /// <value>
        /// The exceptions.
        /// </value>
        [JsonProperty(PropertyName = "exception", NullValueHandling = NullValueHandling.Ignore)]
        public List<SentryException> Exceptions { get; set; }

        /// <summary>
        /// An arbitrary mapping of additional metadata to store with the event.
        /// </summary>
        [JsonProperty(PropertyName = "extra", NullValueHandling = NullValueHandling.Ignore)]
        public object Extra { get; set; }

        /// <summary>
        /// The record severity.
        /// Defaults to error.
        /// </summary>
        [JsonProperty(PropertyName = "level", NullValueHandling = NullValueHandling.Ignore, Required = Required.Always)]
        [JsonConverter(typeof(ErrorLevelConverter))]
        public ErrorLevel Level { get; set; }

        /// <summary>
        /// The name of the logger which created the record.
        /// If missing, defaults to the string root.
        /// 
        /// Ex: "my.logger.name"
        /// </summary>
        [JsonProperty(PropertyName = "logger", NullValueHandling = NullValueHandling.Ignore)]
        public string Logger { get; set; }

        /// <summary>
        /// User-readable representation of this event
        /// </summary>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message { get; set; }

        /// <summary>
        /// Optional Message with arguments.
        /// </summary>
        [JsonProperty(PropertyName = "sentry.interfaces.Message", NullValueHandling = NullValueHandling.Ignore)]
        public SentryMessage MessageObject { get; set; }

        /// <summary>
        /// A list of relevant modules (libraries) and their versions.
        /// Automated to report all modules currently loaded in project.
        /// </summary>
        /// <value>
        /// The modules.
        /// </value>
        [JsonProperty(PropertyName = "modules", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Modules { get; set; }

        /// <summary>
        /// A string representing the platform the client is submitting from. 
        /// This will be used by the Sentry interface to customize various components in the interface.
        /// </summary>
        [JsonProperty(PropertyName = "platform", NullValueHandling = NullValueHandling.Ignore)]
        public string Platform { get; set; }

        /// <summary>
        /// String value representing the project
        /// </summary>
        [JsonProperty(PropertyName = "project", NullValueHandling = NullValueHandling.Ignore)]
        public string Project { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SentryRequest"/> object, containing information about the HTTP request.
        /// </summary>
        /// <value>
        /// The <see cref="SentryRequest"/> object, containing information about the HTTP request.
        /// </value>
        [JsonProperty(PropertyName = "request", NullValueHandling = NullValueHandling.Ignore)]
        public SentryRequest Request { get; set; }

        /// <summary>
        /// Identifies the host client from which the event was recorded.
        /// </summary>
        [JsonProperty(PropertyName = "server_name", NullValueHandling = NullValueHandling.Ignore)]
        public string ServerName { get; set; }

        /// <summary>
        /// A map or list of tags for this event.
        /// </summary>
        [JsonProperty(PropertyName = "tags", NullValueHandling = NullValueHandling.Ignore)]
        public IDictionary<string, string> Tags { get; set; }

        /// <summary>
        /// Indicates when the logging record was created (in the Sentry client).
        /// Defaults to DateTime.UtcNow()
        /// </summary>
        [JsonProperty(PropertyName = "timestamp", NullValueHandling = NullValueHandling.Ignore)]
        public DateTime TimeStamp { get; set; }

        /// <summary>
        /// Gets or sets the <see cref="SentryUser"/> object, which describes the authenticated User for a request.
        /// </summary>
        /// <value>
        /// The <see cref="SentryUser"/> object, which describes the authenticated User for a request.
        /// </value>
        [JsonProperty(PropertyName = "user", NullValueHandling = NullValueHandling.Ignore)]
        public SentryUser User { get; set; }


        /// <summary>
        /// Converts the <see cref="JsonPacket" /> into a JSON string.
        /// </summary>
        /// <param name="formatting">The formatting.</param>
        /// <returns>
        /// The <see cref="JsonPacket" /> as a JSON string.
        /// </returns>
        public virtual string ToString(Formatting formatting)
        {
            return JsonConvert.SerializeObject(this, formatting);
        }


        /// <summary>
        /// Converts the <see cref="JsonPacket"/> into a JSON string.
        /// </summary>
        /// <returns>
        /// The <see cref="JsonPacket"/> as a JSON string.
        /// </returns>
        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}