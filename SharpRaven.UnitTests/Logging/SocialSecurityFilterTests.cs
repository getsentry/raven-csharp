using System;

using NUnit.Framework;

using SharpRaven.Logging.Filters;

namespace SharpRaven.UnitTests.Logging
{
    [TestFixture]
    [Ignore("Not implemented yet")]
    public class SocialSecurityFilterTests
    {
        [Test]
        public void InvalidSocialSecurityNumber_IsNotScrubbed()
        {
            const string socialSecurityNumber = "1531";
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                socialSecurityNumber);

            SocialSecurityFilter filter = new SocialSecurityFilter();
            var output = filter.Filter(input);

            Assert.That(output, Is.StringContaining(socialSecurityNumber));
        }


        [Test]
        public void ValidSocialSecurityNumber_IsScrubbed()
        {
            const string socialSecurityNumber = "55518231234";
            var input = String.Format(
                "Lorem ipsum dolor sit amet, consectetur adipiscing elit. {0} Praesent est dui, ornare eget condimentum a, tincidunt sit amet lectus. Nulla pellentesque, tortor eget tempus malesuada.",
                socialSecurityNumber);

            SocialSecurityFilter filter = new SocialSecurityFilter();
            var output = filter.Filter(input);

            Assert.That(output, Is.Not.StringContaining(socialSecurityNumber));
        }
    }
}