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
using System.Text;

#endregion

using System;
using System.Collections.Generic;
using System.Net;
#if NET35
using System.Web;
#endif

namespace SharpRaven.Data
{
    /// <summary>
    /// Represents the UserFeedback that is transmitted to Sentry
    /// </summary>
    public class SentryUserFeedback
    {
        /// <summary>
        /// The name associated with this feedback
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// The email associated with this feedback
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        /// The comments associated with this feedback
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// The event ID associated with this feedback
        /// </summary>
        public string EventID {get; set;}

        /// <summary>
        /// Returns the url request string for this user feedback
        /// </summary>
        /// <returns>A <see cref="System.String"/> that represents the url request string for this <see cref="SharpRaven.Data.SentryUserFeedback"/>.</returns>
        public override string ToString()
        {
			return string.Format("eventId={0}&name={1}&email={2}&comments={3}",
#if NET35
                                 HttpUtility.UrlEncode(EventID),
                                 HttpUtility.UrlEncode(Name),
                                 HttpUtility.UrlEncode(Email),
                                 HttpUtility.UrlEncode(Comments));
#elif NET40
                                 WebUtility.HtmlEncode(EventID),
                                 WebUtility.HtmlEncode(Name),
                                 WebUtility.HtmlEncode(Email),
                                 WebUtility.HtmlEncode(Comments));
#else
                                 WebUtility.UrlEncode(EventID), 
			                     WebUtility.UrlEncode(Name), 
			                     WebUtility.UrlEncode(Email), 
			                     WebUtility.UrlEncode(Comments));
#endif
        }
    }
}

