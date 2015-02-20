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
using System.Text.RegularExpressions;

namespace SharpRaven.Logging.Filters
{
    /// <summary>
    /// The credit card filter.
    /// </summary>
    public class CreditCardFilter : IFilter
    {
        /// <summary>
        /// Filters credit card numbers from the input.
        /// </summary>
        /// <param name="input">
        /// The input.
        /// </param>
        /// <returns>
        /// The <see cref="string"/> with credit card numbers removed.
        /// </returns>
        public string Filter(string input)
        {
            Regex cardRegex = new Regex(@"\b(?:\d[ -]*?){13,16}\b", RegexOptions.IgnoreCase);

            return cardRegex.Replace(input, m => IsValidCreditCardNumber(m.Value) ? "####-CC-TRUNCATED-####" : m.Value);
        }


        /// <summary>
        /// Validates a credit card number using Luhn algorithm.
        /// Extremely fast Luhn algorithm implementation, based on 
        /// pseudo code from Cliff L. Biffle (http://microcoder.livejournal.com/17175.html)
        /// Copyleft Thomas @ Orb of Knowledge:
        /// http://orb-of-knowledge.blogspot.com/2009/08/extremely-fast-luhn-function-for-c.html
        /// </summary>
        /// <param name="number">
        /// The credit card number.
        /// </param>
        /// <returns>
        /// True if a valid credit card number; otherwise false.
        /// </returns>
        private bool IsValidCreditCardNumber(string number)
        {
            number = number.Replace("-", String.Empty);
            number = number.Replace(" ", String.Empty);

            int[] deltas = new[] { 0, 1, 2, 3, 4, -4, -3, -2, -1, 0 };
            int checksum = 0;
            char[] chars = number.ToCharArray();

            for (int i = chars.Length - 1; i > -1; i--)
            {
                int j = chars[i] - 48;
                checksum += j;

                if (((i - chars.Length) % 2) == 0)
                    checksum += deltas[j];
            }

            return (checksum % 10) == 0;
        }
    }
}