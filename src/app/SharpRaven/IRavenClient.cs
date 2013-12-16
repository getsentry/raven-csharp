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
using System.Collections.Generic;

using SharpRaven.Data;
using SharpRaven.Logging;

namespace SharpRaven
{
    /// <summary>
    /// Raven client interface.
    /// </summary>
    public interface IRavenClient
    {
        /// <summary>
        /// The Dsn currently being used to log exceptions.
        /// </summary>
        Dsn CurrentDsn { get; set; }

        /// <summary>
        /// Interface for providing a 'log scrubber' that removes 
        /// sensitive information from exceptions sent to sentry.
        /// </summary>
        IScrubber LogScrubber { get; set; }

        /// <summary>
        /// Enable Gzip Compression?
        /// Defaults to <c>true</c>.
        /// </summary>
        bool Compression { get; set; }

        /// <summary>
        /// Logger. Default is "root"
        /// </summary>
        string Logger { get; set; }


        /// <summary>
        /// Captures the exception.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns></returns>
        int CaptureException(Exception e, IDictionary<string, string> tags = null, object extra = null);


        /// <summary>
        /// Captures the message.
        /// </summary>
        /// <param name="message">The message to capture.</param>
        /// <param name="level">The <see cref="ErrorLevel"/> of the captured message.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <param name="extra">The extra metadata to send with the captured exception.</param>
        /// <returns></returns>
        int CaptureMessage(string message,
                           ErrorLevel level = ErrorLevel.Info,
                           Dictionary<string, string> tags = null,
                           object extra = null);


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <returns></returns>
        [Obsolete("The more common CaptureException method should be used")]
        int CaptureEvent(Exception e);


        /// <summary>
        /// Captures the event.
        /// </summary>
        /// <param name="e">The <see cref="Exception"/> to capture.</param>
        /// <param name="tags">The tags to annotate the captured exception with.</param>
        /// <returns></returns>
        [Obsolete("The more common CaptureException method should be used")]
        int CaptureEvent(Exception e, Dictionary<string, string> tags);
    }
}