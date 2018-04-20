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
using Newtonsoft.Json;
using SharpRaven.Utilities;

namespace SharpRaven.Data.Context
{
    /// <summary>
    /// This describes a runtime in more detail.
    /// </summary>
    /// <remarks>
    /// Typically this context is used multiple times if multiple runtimes are involved (for instance if you have a JavaScript application running on top of JVM)
    /// </remarks>
    /// <seealso href="https://docs.sentry.io/clientdev/interfaces/contexts/"/>
    public class Runtime
    {
        /// <summary>
        /// The name of the runtime.
        /// </summary>
        [JsonProperty(PropertyName = "name", NullValueHandling = NullValueHandling.Ignore)]
        public string Name { get; set; }
        /// <summary>
        /// The version identifier of the runtime.
        /// </summary>
        [JsonProperty(PropertyName = "version", NullValueHandling = NullValueHandling.Ignore)]
        public string Version { get; set; }

        /// <summary>
        /// Clones this instance
        /// </summary>
        /// <returns></returns>
        internal Runtime Clone()
        {
            return new Runtime
            {
                Name = this.Name,
                Version = this.Version
            };
        }

        /// <summary>
        /// Creates a Runtime instance while loading relevant data
        /// </summary>
        /// <returns>An instance if any property was successfuly set or null otherwise.</returns>
        public static Runtime Create()
        {
            try
            {
                var runtime = new Runtime
                {
                    Name = RuntimeInfoHelper.GetRuntimeVersion()
                };

                return runtime;
            }
            catch (Exception e)
            {
                SystemUtil.WriteError(e);
                return null;
            }
        }
    }
}
