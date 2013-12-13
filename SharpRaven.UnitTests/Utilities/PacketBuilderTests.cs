using NUnit.Framework;

using SharpRaven.Utilities;

namespace SharpRaven.UnitTests.Utilities
{
    [TestFixture]
    public class PacketBuilderTests
    {
        [Test]
        public void CreateAuthenticationHeader_ReturnsCorrectAuthenticationHeader()
        {
            const string expected =
                @"^Sentry sentry_version=4, sentry_client=SharpRaven/1.0, sentry_timestamp=\d+, sentry_key=7d6466e66155431495bdb4036ba9a04b, sentry_secret=4c1cfeab7ebd4c1cb9e18008173a3630$";

            var dsn = new Dsn(
                "https://7d6466e66155431495bdb4036ba9a04b:4c1cfeab7ebd4c1cb9e18008173a3630@app.getsentry.com/3739");

            var authenticationHeader = PacketBuilder.CreateAuthenticationHeader(dsn);

            Assert.That(authenticationHeader, Is.StringMatching(""));
        }
    }
}