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
    /// Interface for the <c>request</c> object.
    /// </summary>
    public interface ISentryRequest
    {
        /// <summary>
        /// Gets or sets the cookies.
        /// </summary>
        /// <value>
        /// The cookies.
        /// </value>
        IDictionary<string, string> Cookies { get; set; }

        /// <summary>
        /// The data variable should only contain the request body (not the query string). It can either be a dictionary (for standard HTTP requests) or a raw request body.
        /// </summary>
        /// <value>
        /// The data.
        /// </value>
        object Data { get; set; }

        /// <summary>
        /// The env variable is a compounded dictionary of HTTP headers as well as environment information passed from the webserver.
        /// Sentry will explicitly look for REMOTE_ADDR in env for things which require an IP address.
        /// </summary>
        /// <value>
        /// The environment.
        /// </value>
        IDictionary<string, string> Environment { get; set; }

        /// <summary>
        /// Gets or sets the headers.
        /// </summary>
        /// <value>
        /// The headers.
        /// </value>
        IDictionary<string, string> Headers { get; set; }

        /// <summary>
        /// Gets or sets the method of the HTTP request.
        /// </summary>
        /// <value>
        /// The method of the HTTP request.
        /// </value>
        string Method { get; set; }

        /// <summary>
        /// Gets or sets the query string.
        /// </summary>
        /// <value>
        /// The query string.
        /// </value>
        string QueryString { get; set; }

        /// <summary>
        /// Gets or sets the URL of the HTTP request.
        /// </summary>
        /// <value>
        /// The URL of the HTTP request.
        /// </value>
        string Url { get; set; }

        /// <summary>
        /// Gets the user.
        /// </summary>
        /// <returns>
        /// If an HTTP context is available, an instance of <see cref="SentryUser"/>, otherwise <c>null</c>.
        /// </returns>
        SentryUser GetUser();
    }
}