using System;
using System.Collections.Generic;
using Newtonsoft.Json;
using NUnit.Framework;
using SharpRaven.Data.Context;

namespace SharpRaven.UnitTests.Data.Context
{
    [TestFixture]
    public class DeviceTests
    {
        [Test]
        public void Create_Architecture_RetrievedFromEnvironment()
        {
            var device = Device.Create();

            Assert.NotNull(device.Architecture);
            Assert.AreEqual(Device.GetArchitecture(), device.Architecture);
        }

        [Test]
        public void Create_BootTime_SometimeInThePast()
        {
            var device = Device.Create();

            Assert.True(device.BootTime.HasValue);
            Assert.AreNotEqual(default(DateTimeOffset), device.BootTime.Value);
            Assert.Greater(DateTimeOffset.UtcNow, device.BootTime);
        }

        [Test]
        public void Ctor_NoPropertyFilled_SerializesEmptyObject()
        {
            var device = new Device();

            var actual = JsonConvert.SerializeObject(device);

            Assert.That(actual, Is.EqualTo("{}"));
        }

        [Test]
        public void SerializeObject_AllPropertiesSetToNonDefault_SerializesValidObject()
        {
            var device = new Device
            {
                Name = "testing.sentry.io",
                Architecture = "x64",
                BatteryLevel = 99,
                BootTime = DateTimeOffset.MaxValue,
                ExternalFreeStorage = 100_000_000_000_000, // 100 TB
                ExternalStorageSize = 1_000_000_000_000_000, // 1 PB
                Family = "Windows",
                FreeMemory = 200_000_000_000, // 200 GB
                MemorySize = 500_000_000_000, // 500 GB
                StorageSize = 100_000_000,
                FreeStorage = 0,
                Model = "Windows Server 2012 R2",
                ModelId = "0921309128012",
                Orientation = DeviceOrientation.Portrait,
                Simulator = false,
                Timezone = TimeZoneInfo.Local,
                UsableMemory = 100
            };

            var actual = JsonConvert.SerializeObject(device);

            Assert.That(actual, Is.EqualTo(
                         $"{{\"timezone\":\"{TimeZoneInfo.Local.Id}\"," +
                         "\"name\":\"testing.sentry.io\"," +
                         "\"family\":\"Windows\"," +
                         "\"model\":\"Windows Server 2012 R2\"," +
                         "\"model_id\":\"0921309128012\"," +
                         "\"arch\":\"x64\"," +
                         "\"battery_level\":99," +
                         "\"orientation\":\"portrait\"," +
                         "\"simulator\":false," +
                         "\"memory_size\":500000000000," +
                         "\"free_memory\":200000000000," +
                         "\"usable_memory\":100," +
                         "\"storage_size\":100000000," +
                         "\"free_storage\":0," +
                         "\"external_storage_size\":1000000000000000," +
                         "\"external_free_storage\":100000000000000," +
                         "\"boot_time\":\"9999-12-31T23:59:59.9999999+00:00\"}"));
        }

        [Theory]
        [Test, TestCaseSource(typeof(DeviceTests), nameof(TestCases))]
        public void SerializeObject_TestCase_SerializesAsExpected(TestCase @case)
        {
            var actual = JsonConvert.SerializeObject(@case.Object);

            Assert.That(actual, Is.EqualTo(@case.ExpectedSerializationOutput));
        }

        public static IEnumerable<object[]> TestCases()
        {
            yield return new object[] { new TestCase
            {
                Object = new Device(),
                ExpectedSerializationOutput = "{}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { Name = "some name" },
                ExpectedSerializationOutput = "{\"name\":\"some name\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { Orientation = DeviceOrientation.Landscape },
                ExpectedSerializationOutput = "{\"orientation\":\"landscape\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { Family = "some family"},
                ExpectedSerializationOutput = "{\"family\":\"some family\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { Model = "some model"},
                ExpectedSerializationOutput = "{\"model\":\"some model\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { ModelId = "some model id"},
                ExpectedSerializationOutput = "{\"model_id\":\"some model id\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { Architecture = "some arch"},
                ExpectedSerializationOutput = "{\"arch\":\"some arch\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { BatteryLevel = 1},
                ExpectedSerializationOutput = "{\"battery_level\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { Simulator = false},
                ExpectedSerializationOutput = "{\"simulator\":false}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { MemorySize = 1},
                ExpectedSerializationOutput = "{\"memory_size\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { FreeMemory = 1},
                ExpectedSerializationOutput = "{\"free_memory\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { UsableMemory = 1},
                ExpectedSerializationOutput = "{\"usable_memory\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { StorageSize = 1},
                ExpectedSerializationOutput = "{\"storage_size\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { FreeStorage = 1},
                ExpectedSerializationOutput = "{\"free_storage\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { ExternalStorageSize = 1},
                ExpectedSerializationOutput = "{\"external_storage_size\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { ExternalFreeStorage = 1},
                ExpectedSerializationOutput = "{\"external_free_storage\":1}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { BootTime = DateTimeOffset.MaxValue},
                ExpectedSerializationOutput = "{\"boot_time\":\"9999-12-31T23:59:59.9999999+00:00\"}"
            }};

            yield return new object[] { new TestCase
            {
                Object = new Device { Timezone = TimeZoneInfo.Utc},
                ExpectedSerializationOutput = "{\"timezone\":\"UTC\"}"
            }};
        }
    }
}
