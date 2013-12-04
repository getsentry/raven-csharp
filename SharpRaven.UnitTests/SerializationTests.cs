using System;
using System.Collections.Generic;

using NUnit.Framework;

using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using SharpRaven.Data;

namespace SharpRaven.UnitTests
{
    [TestFixture]
    public class SerializationTests
    {
        [Test]
        public void SerializedJsonPacket_WithValidData_IsValid()
        {
            var exception = TestHelper.GetException();

            // TODO: This packet should preferably be "complete", i.e. contain as much information as possible. --asbjornu
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
            JsonSchema schema = SchemaHelper.GetSchema();

            jPacket.Validate(schema, (s, e) => Console.WriteLine(e.Message));
            Console.WriteLine(jPacket);

            var valid = jPacket.IsValid(schema);

            Assert.That(valid, Is.True);
        }
    }
}