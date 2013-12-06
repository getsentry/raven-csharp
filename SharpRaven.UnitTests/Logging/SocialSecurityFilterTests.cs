using NUnit.Framework;

using SharpRaven.Logging.Filters;

namespace SharpRaven.UnitTests.Logging
{
    [TestFixture]
    [Ignore("Not implemented yet")]
    public class SocialSecurityFilterTests : FilterTestsBase<SocialSecurityFilter>
    {
        [Test]
        public void InvalidSocialSecurityNumber_IsNotScrubbed()
        {
            InvalidValueIsNotScrubbed("1531");
        }


        [Test]
        public void ValidSocialSecurityNumber_IsScrubbed()
        {
            ValidValueIsScrubbed("55518231234");
        }
    }
}