using NUnit.Framework;

using SharpRaven.Logging.Filters;

namespace SharpRaven.UnitTests.Logging
{
    [TestFixture]
    public class PhoneNumberFilterTests : FilterTestsBase<PhoneNumberFilter>
    {
        [Test]
        public void InvalidPhoneNumber_IsNotScrubbed()
        {
            base.InvalidValueIsNotScrubbed("1531");
        }


        [Test]
        public void ValidPhoneNumber_IsScrubbed()
        {
            ValidValueIsScrubbed("55518231234");
        }
    }
}