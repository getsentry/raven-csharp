#region License

// Copyright (c) 2013 The Sentry Team and individual contributors.
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
using System.Linq;

using Newtonsoft.Json;

namespace SharpRaven.Data
{
    /// <summary>
    /// Captures a <see cref="string"/> message, optionally formatted with arguments,
    /// as sent to Sentry.
    /// </summary>
    public class SentryMessage
    {
        private readonly string message;
        private readonly object[] parameters;


        /// <summary>
        /// Initializes a new instance of the <see cref="SentryMessage"/> class.
        /// </summary>
        /// <param name="format">The format.</param>
        /// <param name="parameters">The arguments.</param>
        public SentryMessage(string format, params object[] parameters)
            : this(format)
        {
            this.parameters = parameters;
        }


        /// <summary>
        /// Initializes a new instance of the <see cref="SentryMessage"/> class.
        /// </summary>
        /// <param name="message">The message.</param>
        public SentryMessage(string message)
        {
            this.message = message;
        }


        /// <summary>
        /// Gets the message.
        /// </summary>
        /// <value>
        /// The message.
        /// </value>
        [JsonProperty(PropertyName = "message", NullValueHandling = NullValueHandling.Ignore)]
        public string Message
        {
            get { return this.message; }
        }


        /// <summary>
        /// Gets the arguments.
        /// </summary>
        /// <value>
        /// The arguments.
        /// </value>
        [JsonProperty(PropertyName = "params", NullValueHandling = NullValueHandling.Ignore)]
        public object[] Parameters
        {
            get { return this.parameters; }
        }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (this.message != null && this.parameters != null && this.parameters.Any())
                return String.Format(this.message, this.parameters);

            return this.message ?? String.Empty;
        }


        /// <summary>
        /// Implicitly converts the <see cref="string"/> <paramref name="message"/> to a <see cref="SentryMessage"/> object.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The <see cref="string"/> <paramref name="message"/> implicitly converted to a <see cref="SentryMessage"/> object.
        /// </returns>
        public static implicit operator SentryMessage(string message)
        {
            return message != null
                       ? new SentryMessage(message)
                       : null;
        }


        /// <summary>
        /// Implicitly converts the <see cref="SentryMessage"/> object to a <see cref="string"/>.
        /// </summary>
        /// <param name="message">The message.</param>
        /// <returns>
        /// The <see cref="SentryMessage"/> object implicitly converted to a <see cref="string"/>.
        /// </returns>
        public static implicit operator string(SentryMessage message)
        {
            return message != null
                       ? message.ToString()
                       : null;
        }
    }
}