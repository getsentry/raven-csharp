using System;

using NUnit.Framework;

using SharpRaven.Logging.Filters;

namespace SharpRaven.UnitTests.Logging
{
    [TestFixture]
    public class CreditCardFilterTests
    {
        [Test]
        public void InvalidCreditCardNumber_IsNotScrubbed()
        {
            const string creditCardNumber = "1234-5678-9101-1121";
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                creditCardNumber);

            CreditCardFilter filter = new CreditCardFilter();
            var output = filter.Filter(input);

            Assert.That(output, Is.StringContaining(creditCardNumber));
        }


        [Test]
        public void ValidCreditCardNumber_IsScrubbed()
        {
            const string creditCardNumber = "5271-1902-4264-3112";
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                creditCardNumber);

            CreditCardFilter filter = new CreditCardFilter();
            var output = filter.Filter(input);

            Assert.That(output, Is.Not.StringContaining(creditCardNumber));
        }
    }
}