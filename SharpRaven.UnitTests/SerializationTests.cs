using System;
using System.Collections.Generic;
using System.IO;

using NUnit.Framework;

using Newtonsoft.Json;
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


        private static string GetJsonSchemaPath()
        {
            return null;
        }

        private static JsonSchema GetSchema()
        {
            var stream = typeof (SerializationTests).Assembly
                                                    .GetManifestResourceStream(typeof (SerializationTests),
                                                                               "schema.json");

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
            try
            {
                PerformDivideByZero();
            }
            catch (Exception e)
            {
                return e;
            }

            return null;
        }


        [Test]
        [Explicit]
        public void GenerateJsonSchema()
        {
            JsonSchemaGenerator generator = new JsonSchemaGenerator();
            var type = typeof (JsonPacket);
            var schema = generator.Generate(type);
            var path = Path.Combine(typeof (SerializationTests).Assembly.Location, "..", "..");
            path = new DirectoryInfo(path).FullName;
            Console.WriteLine(path);

            using (var writer = new StringWriter())
            using (var jsonTextWriter = new JsonTextWriter(writer))
            {
                schema.WriteTo(jsonTextWriter);
            }
        }


        [Test]
        public void JsonPacket_WithMissingLevel_IsNotValid()
        {
            var exception = GetException();

            JsonPacket packet = new JsonPacket("https://public:secret@app.getsentry.com/1337", exception)
            {
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

            Assert.That(valid, Is.False);
        }


        [Test]
        public void JsonPacket_WithValidData_IsValid()
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