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
using System.Linq;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

namespace SharpRaven.Data
{
    /// <summary>
    /// Represents Sentry's version of an <see cref="Exception"/>'s stack trace.
    /// </summary>
    public class SentryStacktrace
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SentryStacktrace"/> class.
        /// </summary>
        /// <param name="exception">The <see cref="Exception"/>.</param>
        /// <param name="forceCaptureStackTrace">Force to capture a full StackTrace if no stack frame can be extract from exception</param>
        public SentryStacktrace(Exception exception, bool forceCaptureStackTrace = false)
        {
            if (exception == null)
                return;

            var trace = new StackTrace(exception, true);
            
            var frames = trace.GetFrames();

            if ((frames == null || frames.Length == 0) && forceCaptureStackTrace)
            {
                var currentAssembly = Assembly.GetAssembly(typeof(SentryStacktrace));
                // Skip first frame if called from this assembly (in case of force capture stacktrace)
                frames = new StackTrace(true).GetFrames().SkipWhile(f => f.GetMethod().DeclaringType != null && f.GetMethod().DeclaringType.Assembly == currentAssembly).ToArray();
            }

            if(frames == null || frames.Length == 0)
                return;

            // Sentry expects the frames to be sent in reversed order 
            Frames = frames.Reverse().Select(f => new ExceptionFrame(f)).ToArray();
        }


        /// <summary>
        /// Gets or sets the <see cref="Exception"/> frames.
        /// </summary>
        /// <value>
        /// The <see cref="Exception"/> frames.
        /// </value>
        [JsonProperty(PropertyName = "frames")]
        public ExceptionFrame[] Frames { get; set; }


        /// <summary>
        /// Returns a <see cref="System.String" /> that represents this instance.
        /// </summary>
        /// <returns>
        /// A <see cref="System.String" /> that represents this instance.
        /// </returns>
        public override string ToString()
        {
            if (Frames == null || !Frames.Any())
                return String.Empty;

            StringBuilder sb = new StringBuilder();

            // Have to reverse the frames before presenting them 
            // since they are stored in reversed order.
            foreach (var frame in Frames.Reverse())
            {
                sb.Append("   at ");
                sb.Append(frame);
                sb.AppendLine();
            }

            return sb.ToString();
        }
    }
}