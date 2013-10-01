using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using SharpRaven.Data;

namespace SharpRaven.UnitTests
{
    [TestFixture]
    public class SerializationTests
    {
        private static void PerformDivideByZero()
        {
            int i2 = 0;
            int i = 10 / i2;
        }


        private static JsonSchema GetSchema()
        {
            var stream =
                typeof (SerializationTests).Assembly.GetManifestResourceStream("SharpRaven.UnitTests.schema.json");

            if (stream == null)
            {
                return null;
            }

            using (StreamReader reader = new StreamReader(stream))
            {
                return JsonSchema.Parse(reader.ReadToEnd());
            }
        }


        private static Exception GetException()
        {
            Exception exception = null;

            try
            {
                PerformDivideByZero();
            }
            catch (Exception e)
            {
                exception = e;
            }
            return exception;
        }


        [Test]
        public void JsonPacket_ToString_ReturnsExpectedJsonString()
        {
            var exception = GetException();

            JsonPacket packet = new JsonPacket("https://public:secret@app.getsentry.com/1337", exception)
            {
                Level = ErrorLevel.Fatal,
                Tags = new Dictionary<string, string>
                {
                    { "key1", "value1" },
                    { "key2", "value2" },
                },
            };

            JObject jPacket = JObject.Parse(packet.ToString());
            JsonSchema schema = GetSchema();

            Console.WriteLine(jPacket);

            var valid = jPacket.IsValid(schema);

            Assert.That(valid, Is.True);
        }
    }
}