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

namespace SharpRaven.Data
{
    /// <summary>
    /// Indicates the severity of the error.
    /// </summary>
    public enum ErrorLevel
    {
        /// <summary>
        /// The error is fatal and more severe than a captured exception or regular <see cref="Error"/>. Errors at this severity
        /// will show up as dark red in the Sentry Stream.
        /// </summary>
        Fatal,

        /// <summary>
        /// The error is of the same severity as a captured exception. Errors at this severity
        /// will show up as bright red in the Sentry Stream.
        /// </summary>
        Error,

        /// <summary>
        /// The error is less severe than an a regular <see cref="Error"/>. Errors at this severity will show
        /// up as orange in the Sentry Stream.
        /// </summary>
        Warning,

        /// <summary>
        /// The error is less severe than a <see cref="Warning"/> and is probably expected. Errors at this severity
        /// will show up as blue in the Sentry Stream.
        /// </summary>
        Info,

        /// <summary>
        /// The error is less even severe than an <see cref="Info"/> and is just captured for debug purposes. Errors
        /// at this severity will show up as grey in the Sentry Stream.
        /// </summary>
        Debug
    }
}