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
using System.Linq;

using SharpRaven.Logging.Filters;

namespace SharpRaven.Logging
{
    /// <summary>
    /// Scrubs a JSON packet for sensitive information with <see cref="CreditCardFilter"/>, <see cref="PhoneNumberFilter"/> and <see cref="SocialSecurityFilter"/>.
    /// </summary>
    public class LogScrubber : IScrubber
    {
        private readonly List<IFilter> filters;


        /// <summary>
        /// Initializes a new instance of the <see cref="LogScrubber"/> class.
        /// </summary>
        public LogScrubber()
        {
            this.filters = new List<IFilter>
            {
                new CreditCardFilter(),
                new PhoneNumberFilter(),
                new SocialSecurityFilter()
            };
        }


        /// <summary>
        /// Gets the list of filters 
        /// </summary>
        /// <value>
        /// The filters.
        /// </value>
        public List<IFilter> Filters
        {
            get { return this.filters; }
        }


        /// <summary>
        /// The main interface for scrubbing a JSON packet,
        /// called before compression (if enabled)
        /// </summary>
        /// <param name="input">The serialized JSON packet is given here.</param>
        /// <returns>
        /// Scrubbed JSON packet.
        /// </returns>
        public string Scrub(string input)
        {
            return !String.IsNullOrWhiteSpace(input)
                       ? this.filters.Aggregate(input, (current, f) => f.Filter(current))
                       : input;
        }
    }
}