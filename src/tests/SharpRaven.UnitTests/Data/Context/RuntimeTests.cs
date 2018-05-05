using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpRaven.Data.Context;
using SharpRaven.Utilities;

namespace SharpRaven.UnitTests.Data.Context
{
    [TestFixture]
    public class RuntimeTests
    {
        [Test]
        public void Create_Runtime_NotNullAndAsHelper()
        {
            var actual = Runtime.Create();

            var expected = RuntimeInfoHelper.GetRuntime();
            Assert.NotNull(actual);
            Assert.AreEqual(expected.Build, actual.Build);
            Assert.AreEqual(expected.Version, actual.Version);
            Assert.AreEqual(expected.Name, actual.Name);
            Assert.AreEqual(expected.RawDescription, actual.RawDescription);
        }

        [Test]
        public void Ctor_NoPropertyFilled_SerializesEmptyObject()
        {
            var runtime = new Runtime();

            var actual = JsonConvert.SerializeObject(runtime);

            Assert.That(actual, Is.EqualTo("{}"));
        }

        [Test]
        public void SerializeObject_AllPropertiesSetToNonDefault_SerializesValidObject()
        {
            var runtime = new Runtime
            {
                Version = "2.0.1",
                Name = "NETCore",
                RawDescription = "NETCore 2.0.1"
            };

            var actual = JsonConvert.SerializeObject(runtime);

            Assert.That(actual, Is.EqualTo("{\"name\":\"NETCore\",\"version\":\"2.0.1\",\"raw_description\":\"NETCore 2.0.1\"}"));
        }

        [Test, TestCaseSource(typeof(RuntimeTests), nameof(TestCases))]
        public void SerializeObject_TestCase_SerializesAsExpected(TestCase @case)
        {
            var actual = JsonConvert.SerializeObject(@case.Object);

            Assert.That(actual, Is.EqualTo(@case.ExpectedSerializationOutput));
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { new TestCase
            {
                Object = new Runtime(),
                ExpectedSerializationOutput = "{}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Runtime { Name = "some name" },
                ExpectedSerializationOutput = "{\"name\":\"some name\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Runtime { Version = "some version" },
                ExpectedSerializationOutput = "{\"version\":\"some version\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Runtime { RawDescription = "some Name, some version" },
                ExpectedSerializationOutput = "{\"raw_description\":\"some Name, some version\"}"
            }};
        }
    }
}
