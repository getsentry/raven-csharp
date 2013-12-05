using System;

using NUnit.Framework;

using SharpRaven.Logging.Filters;

namespace SharpRaven.UnitTests.Logging
{
    [TestFixture]
    public class PhoneNumberFilterTests
    {
        [Test]
        public void InvalidPhoneNumber_IsNotScrubbed()
        {
            const string phoneNumber = "1531";
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                phoneNumber);

            PhoneNumberFilter filter = new PhoneNumberFilter();
            var output = filter.Filter(input);

            Assert.That(output, Is.StringContaining(phoneNumber));
        }


        [Test]
        public void ValidPhoneNumber_IsScrubbed()
        {
            const string phoneNumber = "55518231234";
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                phoneNumber);

            PhoneNumberFilter filter = new PhoneNumberFilter();
            var output = filter.Filter(input);

            Assert.That(output, Is.Not.StringContaining(phoneNumber));
        }
    }
}