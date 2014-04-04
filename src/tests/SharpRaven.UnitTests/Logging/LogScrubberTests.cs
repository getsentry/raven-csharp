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
using System.Linq;

using NUnit.Framework;

using SharpRaven.Logging;
using SharpRaven.Logging.Filters;

namespace SharpRaven.UnitTests.Logging
{
    [TestFixture]
    public class @LogScrubberTests
    {
        #region Setup/Teardown

        [SetUp]
        public void SetUp()
        {
            this.scrubber = new LogScrubber();
        }

        #endregion

        private LogScrubber scrubber;


        [Test]
        public void Filters_ContainCreditCardFilter()
        {
            Assert.That(this.scrubber.Filters.OfType<CreditCardFilter>().ToArray(),
                        Has.Length.EqualTo(1));
        }


        [Test]
        public void Filters_ContainPhoneNumberFilter()
        {
            Assert.That(this.scrubber.Filters.OfType<PhoneNumberFilter>().ToArray(),
                        Has.Length.EqualTo(1));
        }


        [Test]
        public void Filters_ContainSocialSecurityFilter()
        {
            Assert.That(this.scrubber.Filters.OfType<SocialSecurityFilter>().ToArray(),
                        Has.Length.EqualTo(1));
        }


        [Test]
        public void Filters_HasExpectedCount()
        {
            Assert.That(this.scrubber.Filters, Has.Count.EqualTo(3));
        }


        [Test]
        public void InvalidValues_AreNotScrubbed()
        {
            const string invalidCreditCardNumber = "1234-5678-9101-1121";
            const string invalidPhoneNumber = "1531";

            // TODO: Add social security number test data once that filter is implemented.

            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, {1} tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                invalidCreditCardNumber,
                invalidPhoneNumber);

            var output = this.scrubber.Scrub(input);

            Assert.That(output, Is.StringContaining(invalidCreditCardNumber));
            Assert.That(output, Is.StringContaining(invalidPhoneNumber));
        }


        [Test]
        public void Scrub_Null_ReturnsNull()
        {
            var result = this.scrubber.Scrub(null);

            Assert.That(result, Is.Null);
        }


        [Test]
        public void ValidValues_AreScrubbed()
        {
            const string validCreditCardNumber = "5271-1902-4264-3112";
            const string validPhoneNumber = "55518231234";

            // TODO: Add social security number test data once that filter is implemented.

            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, {1} tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                validCreditCardNumber,
                validPhoneNumber);

            var output = this.scrubber.Scrub(input);

            Assert.That(output, Is.Not.StringContaining(validCreditCardNumber));
            Assert.That(output, Is.Not.StringContaining(validPhoneNumber));
        }
    }
}