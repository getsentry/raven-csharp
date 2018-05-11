using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using Newtonsoft.Json;
using NUnit.Framework;
using SharpRaven.Data.Context;

namespace SharpRaven.UnitTests.Data.Context
{
    [TestFixture]
    public class AppTests
    {
        [Test]
        public void Create_StartTime_SometimeInThePast()
        {
            var app = App.Create();

            Assert.That(app.StartTime.HasValue, Is.True);
            Assert.That(app.StartTime.Value, Is.Not.EqualTo(default(DateTimeOffset)));
            Assert.That(app.StartTime, Is.LessThan(DateTimeOffset.UtcNow));
        }

#if !NET35
        [Test]
        public void Create_Version_EqualEntryAssembly()
        {
            var app = App.Create();

            Assert.That(app.Version, Is.Not.Null);
            Assert.AreEqual(typeof(AppTests).Assembly.GetName().Version.ToString(), app.Version);
        }

        [Test]
        public void Create_Name_EqualEntryAssembly()
        {
            var app = App.Create();

            Assert.NotNull(app.Name);
            Assert.AreEqual(typeof(AppTests).Assembly.GetName().Name, app.Name);
        }
#endif

        [Test]
        public void Ctor_NoPropertyFilled_SerializesEmptyObject()
        {
            var app = new App();

            var actual = JsonConvert.SerializeObject(app);

            Assert.That(actual, Is.EqualTo("{}"));
        }

        [Test]
        public void SerializeObject_AllPropertiesSetToNonDefault_SerializesValidObject()
        {
            var app = new App
            {
                Version = "8b03fd7",
                Build = "1.23152",
                BuildType = "nightly",
                Hash = "93fd0e9a",
                Name = "Sentry.Test.App",
                StartTime = DateTimeOffset.MaxValue
            };

            var actual = JsonConvert.SerializeObject(app);

            Assert.That(actual, Is.EqualTo("{\"app_start_time\":\"9999-12-31T23:59:59.9999999+00:00\","
                                           + "\"device_app_hash\":\"93fd0e9a\","
                                           + "\"build_type\":\"nightly\","
                                           + "\"app_name\":\"Sentry.Test.App\","
                                           + "\"app_version\":\"8b03fd7\","
                                           + "\"app_build\":\"1.23152\"}"));
        }

        [Theory]
        [Test, TestCaseSource(typeof(AppTests), nameof(TestCases))]
        public void SerializeObject_TestCase_SerializesAsExpected(TestCase @case)
        {
            var actual = JsonConvert.SerializeObject(@case.Object);

            Assert.That(actual, Is.EqualTo(@case.ExpectedSerializationOutput));
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { new TestCase
            {
                Object = new App(),
                ExpectedSerializationOutput = "{}"
            }};

            yield return new object[] { new TestCase
                {
                    Object = new App { Name = "some name" },
                    ExpectedSerializationOutput = "{\"app_name\":\"some name\"}"
                }};

            yield return new object[] { new TestCase
            {
                Object = new App { Build = "some build" },
                ExpectedSerializationOutput = "{\"app_build\":\"some build\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new App { BuildType = "some build type" },
                ExpectedSerializationOutput = "{\"build_type\":\"some build type\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new App { Hash = "some hash" },
                ExpectedSerializationOutput = "{\"device_app_hash\":\"some hash\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new App { StartTime = DateTimeOffset.MaxValue },
                ExpectedSerializationOutput = "{\"app_start_time\":\"9999-12-31T23:59:59.9999999+00:00\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new App { Version = "some version" },
                ExpectedSerializationOutput = "{\"app_version\":\"some version\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new App { Identifier = "some identifier" },
                ExpectedSerializationOutput = "{\"app_identifier\":\"some identifier\"}"
            }};
        }
    }
}
