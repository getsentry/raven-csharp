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
        public void Create_Name_NotNullAndAsHelper()
        {
            var runtime = Runtime.Create();

            var expected = RuntimeInfoHelper.GetRuntimeVersion();
            Assert.NotNull(runtime.Name);
            Assert.AreEqual(expected, runtime.Name);
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
            };

            var actual = JsonConvert.SerializeObject(runtime);

            Assert.That(actual, Is.EqualTo("{\"name\":\"NETCore\",\"version\":\"2.0.1\"}"));
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
        }
    }
}
