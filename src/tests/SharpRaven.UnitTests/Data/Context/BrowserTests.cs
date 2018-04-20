using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpRaven.Data.Context;

namespace SharpRaven.UnitTests.Data.Context
{
    [TestFixture]
    public class BrowserTests
    {
        [Test]
        public void SerializeObject_AllPropertiesSetToNonDefault_SerializesValidObject()
        {
            var browser = new Browser
            {
                Version = "6",
                Name = "Internet Explorer",
            };

            var actual = JsonConvert.SerializeObject(browser);

            Assert.That(actual, Is.EqualTo("{\"name\":\"Internet Explorer\",\"version\":\"6\"}"));
        }

        [Test, TestCaseSource(typeof(BrowserTests), nameof(TestCases))]
        public void SerializeObject_TestCase_SerializesAsExpected(TestCase @case)
        {
            var actual = JsonConvert.SerializeObject(@case.Object);

            Assert.That(actual, Is.EqualTo(@case.ExpectedSerializationOutput));
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { new TestCase
            {
                Object = new Browser(),
                ExpectedSerializationOutput = "{}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Browser { Name = "some name" },
                ExpectedSerializationOutput = "{\"name\":\"some name\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Browser { Version = "some version" },
                ExpectedSerializationOutput = "{\"version\":\"some version\"}"
            }};
        }
    }
}
