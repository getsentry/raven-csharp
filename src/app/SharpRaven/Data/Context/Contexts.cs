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

using Newtonsoft.Json;

namespace SharpRaven.Data.Context
{
    /// <summary>
    /// Represents Sentry's structured Context
    /// </summary>
    /// <seealso href="https://docs.sentry.io/clientdev/interfaces/contexts/"/>
    public class Contexts
    {
        [JsonProperty(PropertyName = "app", NullValueHandling = NullValueHandling.Ignore)]
        private App app;
        [JsonProperty(PropertyName = "browser", NullValueHandling = NullValueHandling.Ignore)]
        private Browser browser;
        [JsonProperty(PropertyName = "device", NullValueHandling = NullValueHandling.Ignore)]
        private Device device;
        [JsonProperty(PropertyName = "os", NullValueHandling = NullValueHandling.Ignore)]
        private OperatingSystem operatingSystem;
        [JsonProperty(PropertyName = "runtime", NullValueHandling = NullValueHandling.Ignore)]
        private Runtime runtime;

        /// <summary>
        /// Describes the application.
        /// </summary>
        [JsonIgnore]
        public App App => this.app ?? (this.app = new App());
        /// <summary>
        /// Describes the application.
        /// </summary>
        [JsonIgnore]
        public Browser Browser => this.browser ?? (this.browser = new Browser());
        /// <summary>
        /// Describes the device that caused the event.
        /// </summary>
        [JsonIgnore]
        public Device Device => this.device ?? (this.device = new Device());
        /// <summary>
        /// Defines the operating system that caused the event.
        /// </summary>
        /// <remarks>
        /// In web contexts, this is the operating system of the browser (normally pulled from the User-Agent string).
        /// </remarks>
        [JsonIgnore]
        public OperatingSystem OperatingSystem => this.operatingSystem ?? (this.operatingSystem = new OperatingSystem());
        /// <summary>
        /// This describes a runtime in more detail.
        /// </summary>
        [JsonIgnore]
        public Runtime Runtime => this.runtime ?? (this.runtime = new Runtime());

        /// <summary>
        /// Root context created as a result of fetching system information
        /// </summary>
        /// <remarks>
        /// Creating this instance is expensive. Its reference is not exposed outside this class.
        /// It's a reference to be cloned into new Contexts that can be mutated per transaction
        /// </remarks>
        private static Contexts root;

        /// <summary>
        /// Creates a deep clone of this context
        /// </summary>
        /// <returns></returns>
        internal Contexts Clone()
        {
            return new Contexts
            {
                app = this.app?.Clone(),
                device = this.device?.Clone(),
                operatingSystem = this.operatingSystem?.Clone(),
                runtime = this.runtime?.Clone()
            };
        }

        /// <summary>
        /// Creates a Contexts instance while attempting to load relevant information
        /// </summary>
        /// <returns></returns>
        /// <returns>An new instance of contexts with system information pre-populated</returns>
        internal static Contexts Create() => (root = root ?? CreateNew()).Clone();

        private static Contexts CreateNew() 
            => new Contexts
            {
                app = App.Create(),
                device = Device.Create(),
                operatingSystem = OperatingSystem.Create(),
                runtime = Runtime.Create()
            };
    }
}
