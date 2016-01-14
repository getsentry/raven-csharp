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

namespace SharpRaven.Data
{
    /// <summary>
    /// Represents an event being captured by <see cref="IRavenClient.Capture(SentryEvent)"/>.
    /// </summary>
    public class SentryEvent
    {
        private readonly Exception exception;
        private readonly IList<string> fingerprint;
        private readonly IDictionary<string, string> tags;


        /// <summary>Initializes a new instance of the <see cref="SentryEvent" /> class.</summary>
        /// <param name="exception">The <see cref="Exception" /> to capture.</param>
        public SentryEvent(Exception exception)
            : this()
        {
            this.exception = exception;
            Level = ErrorLevel.Error;
        }


        /// <summary>Initializes a new instance of the <see cref="SentryEvent"/> class.</summary>
        /// <param name="message">The message to capture.</param>
        public SentryEvent(SentryMessage message)
            : this()
        {
            Message = message;
        }


        /// <summary>Prevents a default instance of the <see cref="SentryEvent"/> class from being created.</summary>
        private SentryEvent()
        {
            this.tags = new Dictionary<string, string>();
            this.fingerprint = new List<string>();
        }


        /// <summary>Gets the <see cref="Exception" /> to capture.</summary>
        /// <value>The <see cref="Exception" /> to capture.</value>
        public Exception Exception
        {
            get { return this.exception; }
        }

        /// <summary>Gets extra metadata to send with the captured <see name="Exception" /> or <see cref="Message" />.</summary>
        /// <value>
        /// The extra metadata to send with the captured <see name="Exception" /> or <see cref="Message" />.
        /// </value>
        public object Extra { get; set; }

        /// <summary>
        /// Gets the custom fingerprint to identify the captured <see name="Exception" /> or <see cref="Message" /> with.
        /// </summary>
        /// <value>
        /// The custom fingerprint to identify the captured <see name="Exception" /> or <see cref="Message" /> with.
        /// </value>
        public IList<string> Fingerprint
        {
            get { return this.fingerprint; }
        }

        /// <summary>
        /// Gets the <see cref="ErrorLevel" /> of the captured <see name="Exception" />
        /// or <see cref="Message" />. Default: <see cref="ErrorLevel.Error" />.
        /// </summary>
        /// <value>
        /// The <see cref="ErrorLevel" /> of the captured <see name="Exception" />
        /// or <see cref="Message" />. Default: <see cref="ErrorLevel.Error" />.
        /// </value>
        public ErrorLevel Level { get; set; }

        /// <summary>Gets the optional message to capture instead of the default <see cref="T:Exception.Message" />.</summary>
        /// <value>
        /// The optional message to capture instead of the default <see cref="T:Exception.Message" />.
        /// </value>
        public SentryMessage Message { get; set; }

        /// <summary>Gets the tags to annotate the captured <see name="Exception" /> or <see cref="Message" /> with.</summary>
        /// <value>
        /// The tags to annotate the captured <see name="Exception" /> or <see cref="Message" /> with.
        /// </value>
        public IDictionary<string, string> Tags
        {
            get { return this.tags; }
        }
    }
}