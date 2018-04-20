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
using System.Diagnostics;
using System.Reflection;

using Newtonsoft.Json;

using SharpRaven.Utilities;

namespace SharpRaven.Data.Context
{
    /// <summary>
    /// Describes the application.
    /// </summary>
    /// <remarks>
    /// As opposed to the runtime, this is the actual application that
    /// was running and carries meta data about the current session.
    /// </remarks>
    /// <seealso href="https://docs.sentry.io/clientdev/interfaces/contexts/"/>
    public class App
    {
        /// <summary>
        /// Version-independent application identifier, often a dotted bundle ID.
        /// </summary>
        [JsonProperty(PropertyName = "app_identifier", NullValueHandling = NullValueHandling.Ignore)]
        public string Identifier { get; set; }
        /// <summary>
        /// Formatted UTC timestamp when the application was started by the user.
        /// </summary>
        [JsonProperty(PropertyName = "app_start_time", NullValueHandling = NullValueHandling.Ignore)]
        public DateTimeOffset? StartTime { get; set; }
        /// <summary>
        /// Application specific device identifier.
        /// </summary>
        [JsonProperty(PropertyName = "device_app_hash", NullValueHandling = NullValueHandling.Ignore)]
        public string Hash { get; set; }
        /// <summary>
        /// String identifying the kind of build, e.g. testflight.
        /// </summary>
        [JsonProperty(PropertyName = "build_type", NullValueHandling = NullValueHandling.Ignore)]
        public string BuildType { get; set; }
        /// <summary>
        /// Human readable application name, as it appears on the platform.
        /// </summary>
        [JsonProperty(PropertyName = "app_name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// Human readable application version, as it appears on the platform.
        /// </summary>
        [JsonProperty(PropertyName = "app_version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }
        /// <summary>
        /// Internal build identifier, as it appears on the platform.
        /// </summary>
        [JsonProperty(PropertyName = "app_build", NullValueHandling = NullValueHandling.Ignore)]
        public string Build { get; set; }

        /// <summary>
        /// The application AssemblyName used to report Version and Name
        /// </summary>
        /// <remarks>
        /// If no value is set, the entry assemby's name will be used, if possible.
        /// When not available, a best effort to locate the entrypoint will be made by walking up the stack
        /// </remarks>
        [JsonIgnore]
        public static AssemblyName ApplicationName { get; set; }

        /// <summary>
        /// Clones this instance
        /// </summary>
        /// <returns></returns>
        internal App Clone()
        {
            return new App
            {
                Build = this.Build,
                Version = this.Version,
                Name = this.Name,
                BuildType = this.BuildType,
                Hash = this.Hash,
                Identifier = this.Identifier,
                StartTime = this.StartTime
            };
        }

        /// <summary>
        /// Creates an App instance while loading relevant data at runtime
        /// </summary>
        /// <returns>An instance if any property was successfuly set or null otherwise.</returns>
        internal static App Create()
        {
            try
            {
                var app = new App();

                var asm = ApplicationName ?? EntryAssemblyNameLocator.GetAssemblyName();
                if (asm != null)
                {
                    app.Name = asm.Name;
                    app.Version = asm.Version?.ToString();
                }

                using (var proc = Process.GetCurrentProcess())
                {
                    app.StartTime = proc.StartTime.ToUniversalTime();
                }

                return app;
            }
            catch (Exception e)
            {
                SystemUtil.WriteError(e);
                return null;
            }
        }

    }
}
