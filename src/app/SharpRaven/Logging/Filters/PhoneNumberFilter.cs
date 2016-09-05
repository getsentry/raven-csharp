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

using System.Text.RegularExpressions;

namespace SharpRaven.Logging.Filters
{
    /// <summary>
    /// An <see cref="IFilter"/> implementation for masking phone numbers in a logged message
    /// </summary>
    public class PhoneNumberFilter : IFilter
    {
        private static readonly Regex phoneRegex;

        /// <summary>
        /// Static initalization of the <see cref="PhoneNumberFilter"/> class.
        /// </summary>
        static PhoneNumberFilter()
        {
            phoneRegex = new Regex(@"1?\W*([2-9][0-8]\d)\W*([2-9]\d{2})\W*(\d{4})(\se?x?t?(\d*))?\s+", RegexOptions.Compiled);
        }


        /// <summary>
        /// An <see cref="IFilter"/> implementation for masking phone numbers in a logged 
        /// </summary>
        public string Filter(string input)
        {
            return phoneRegex.Replace(input, "##-PHONE-TRUNC-##");
        }
    }
}