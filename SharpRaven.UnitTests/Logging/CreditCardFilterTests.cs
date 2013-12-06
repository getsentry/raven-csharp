using NUnit.Framework;

using SharpRaven.Logging.Filters;

namespace SharpRaven.UnitTests.Logging
{
    [TestFixture]
    public class CreditCardFilterTests : FilterTestsBase<CreditCardFilter>
    {
        [Test]
        public void InvalidCreditCardNumber_IsNotScrubbed()
        {
            InvalidValueIsNotScrubbed("1234-5678-9101-1121");
        }


        [Test]
        public void ValidCreditCardNumber_IsScrubbed()
        {
            ValidValueIsScrubbed("5271-1902-4264-3112");
        }
    }
}