using Newtonsoft.Json;
using NUnit.Framework;
using SharpRaven.Data.Context;

namespace SharpRaven.UnitTests.Data.Context
{
    [TestFixture]
    public class ContextsTests
    {
        private const string EmptyJsonObject = "{}";

        [Test]
        public void Ctor_NoPropertyFilled_SerializesEmptyObject()
        {
            var contexts = new Contexts();

            var actual = JsonConvert.SerializeObject(contexts);

            Assert.That(actual, Is.EqualTo(EmptyJsonObject));
        }

        [Test]
        public void Ctor_SingleDevicePropertySet_SerializeSingleProperty()
        {
            var contexts = new Contexts();
            contexts.Device.Architecture = "x86";
            var actual = JsonConvert.SerializeObject(contexts);

            Assert.That(actual, Is.EqualTo("{\"device\":{\"arch\":\"x86\"}}"));
        }

        [Test]
        public void Ctor_SingleAppPropertySet_SerializeSingleProperty()
        {
            var contexts = new Contexts();
            contexts.App.Name = "My.App";
            var actual = JsonConvert.SerializeObject(contexts);

            Assert.That(actual, Is.EqualTo("{\"app\":{\"app_name\":\"My.App\"}}"));
        }

        [Test]
        public void Ctor_SingleOsPropertySet_SerializeSingleProperty()
        {
            var contexts = new Contexts();
            contexts.OperatingSystem.Version = "1.1.1.100";
            var actual = JsonConvert.SerializeObject(contexts);

            Assert.That(actual, Is.EqualTo("{\"os\":{\"version\":\"1.1.1.100\"}}"));
        }

        [Test]
        public void Ctor_SingleRuntimePropertySet_SerializeSingleProperty()
        {
            var contexts = new Contexts();
            contexts.Runtime.Version = "2.1.1.100";
            var actual = JsonConvert.SerializeObject(contexts);

            Assert.That(actual, Is.EqualTo("{\"runtime\":{\"version\":\"2.1.1.100\"}}"));
        }

        [Test]
        public void Ctor_SingleBrowserPropertySet_SerializeSingleProperty()
        {
            var contexts = new Contexts();
            contexts.Browser.Name = "Netscape 1";
            var actual = JsonConvert.SerializeObject(contexts);

            Assert.That(actual, Is.EqualTo("{\"browser\":{\"name\":\"Netscape 1\"}}"));
        }

        [Test]
        public void Ctor_SingleOperatingSystemPropertySet_SerializeSingleProperty()
        {
            var contexts = new Contexts();
            contexts.OperatingSystem.Name = "BeOS 1";
            var actual = JsonConvert.SerializeObject(contexts);

            Assert.That(actual, Is.EqualTo("{\"os\":{\"name\":\"BeOS 1\"}}"));
        }
    }
}
