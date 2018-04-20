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
#if HAS_RUNTIME_INFORMATION
using System.Runtime.InteropServices;
#endif
using Newtonsoft.Json;
using SharpRaven.Utilities;

namespace SharpRaven.Data.Context
{
    /// <summary>
    /// Represents Sentry's context for OS
    /// </summary>
    /// <remarks>
    /// Defines the operating system that caused the event. In web contexts, this is the operating system of the browser (normally pulled from the User-Agent string).
    /// </remarks>
    /// <seealso href="https://docs.sentry.io/clientdev/interfaces/contexts/#context-types"/>
    public class OperatingSystem
    {
        /// <summary>
        /// The name of the operating system.
        /// </summary>
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The version of the operating system.
        /// </summary>
        [JsonProperty(PropertyName = "version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }
        /// <summary>
        /// The internal build revision of the operating system.
        /// </summary>
        [JsonProperty(PropertyName = "build", NullValueHandling = NullValueHandling.Ignore)]
        public string Build { get; set; }
        /// <summary>
        ///  If known, this can be an independent kernel version string. Typically
        /// this is something like the entire output of the 'uname' tool.
        /// </summary>
        [JsonProperty(PropertyName = "kernel_version", NullValueHandling = NullValueHandling.Ignore)]
        public string KernelVersion { get; set; }
        /// <summary>
        ///  An optional boolean that defines if the OS has been jailbroken or rooted.
        /// </summary>
        [JsonProperty(PropertyName = "rooted", NullValueHandling = NullValueHandling.Ignore)]
        public bool? Rooted { get; set; }

        /// <summary>
        /// Clones this instance
        /// </summary>
        /// <returns></returns>
        internal OperatingSystem Clone()
        {
            return new OperatingSystem
            {
                Version = this.Version,
                Name = this.Name,
                Build = this.Build,
                KernelVersion = this.KernelVersion,
                Rooted = this.Rooted
            };
        }

        /// <summary>
        /// Creates an OS instance while loading relevant data at runtime
        /// </summary>
        /// <returns>An instance of Device or null if it failed</returns>
        internal static OperatingSystem Create()
        {
            try
            {
                var os = Environment.OSVersion;
#if HAS_RUNTIME_INFORMATION
                // https://github.com/dotnet/corefx/blob/dbb7a3f2d3938b9b888b876ba4b2fd45fdc10985/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.Unix.cs#L25
                // https://github.com/dotnet/corefx/blob/c46e2e98b77d8c5eb2bc147df13b1505cf9c041e/src/System.Runtime.InteropServices.RuntimeInformation/src/System/Runtime/InteropServices/RuntimeInformation/RuntimeInformation.Windows.cs#L22
                var name = RuntimeInformation.OSDescription;
#else
                // https://github.com/dotnet/corefx/blob/fbe2ff101137abed0bc7fa67f40491d277088a79/src/System.Runtime.Extensions/src/System/OperatingSystem.cs#L53
                // Same as RuntimeInformation.OSDescription on Mono: https://github.com/mono/mono/blob/90b49aa3aebb594e0409341f9dca63b74f9df52e/mcs/class/corlib/System.Runtime.InteropServices.RuntimeInformation/RuntimeInformation.cs#L69
                var name = os.VersionString;
#endif

                return new OperatingSystem
                {
                    Name = name,
                    Version = os.Version.ToString()
                };
            }
            catch (Exception e)
            {
                SystemUtil.WriteError(e);
                return null;
            }
        }
    }
}
