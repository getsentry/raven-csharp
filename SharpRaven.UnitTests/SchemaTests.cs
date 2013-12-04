using System;
using System.IO;

using NUnit.Framework;

using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json.Schema;

using SharpRaven.Data;

namespace SharpRaven.UnitTests
{
    [TestFixture]
    public class SchemaTests
    {
        [Test]
        [Explicit("Run to re-generate the schema.json file based on the current type layout of JsonPacket")]
        public void GenerateJsonSchema()
        {
            JsonSchemaGenerator generator = new JsonSchemaGenerator();
            var type = typeof (JsonPacket);
            var schema = generator.Generate(type);
            schema.Title = type.Name;
            schema.Description = "An exception packet for Sentry";
            var path = SchemaHelper.GetSchemaPath();

            using (var fileStream = File.OpenWrite(path))
            using (var fileWriter = new StreamWriter(fileStream))
            using (var jsonTextWriter = new JsonTextWriter(fileWriter))
            {
                jsonTextWriter.Formatting = Formatting.Indented;
                schema.WriteTo(jsonTextWriter);
            }
        }


        [Test]
        public void Validate_JsonPacketWithMissingLevel_IsNotValid()
        {
            const string json = @"{
  ""tags"": {
    ""key1"": ""value1"",
    ""key2"": ""value2""
  },
  ""event_id"": ""8ee5d717693b402d93c04aea0107ea60"",
  ""project"": ""https://public:secret@app.getsentry.com/1337"",
  ""culprit"": ""SharpRaven.UnitTests.TestHelper in PerformDivideByZero"",
  ""timestamp"": ""2013-12-04T09:28:34.7802734Z"",
  ""logger"": ""root"",
  ""platform"": ""csharp"",
  ""message"": ""Attempted to divide by zero."",
  ""server_name"": ""AUL-PC"",
  ""exception"": [
    {
      ""type"": ""DivideByZeroException"",
      ""value"": ""Attempted to divide by zero."",
      ""module"": ""SharpRaven.UnitTests"",
      ""stacktrace"": {
        ""frames"": [
          {
            ""abs_path"": null,
            ""filename"": ""c:\\Users\\aul\\Dev\\Misc\\raven-csharp\\SharpRaven.UnitTests\\TestHelper.cs"",
            ""module"": ""SharpRaven.UnitTests.TestHelper"",
            ""function"": ""GetException"",
            ""vars"": null,
            ""pre_context"": null,
            ""context_line"": ""System.Exception GetException()"",
            ""lineno"": 18,
            ""colno"": 17,
            ""in_app"": false,
            ""post_context"": null
          },
          {
            ""abs_path"": null,
            ""filename"": ""c:\\Users\\aul\\Dev\\Misc\\raven-csharp\\SharpRaven.UnitTests\\TestHelper.cs"",
            ""module"": ""SharpRaven.UnitTests.TestHelper"",
            ""function"": ""PerformDivideByZero"",
            ""vars"": null,
            ""pre_context"": null,
            ""context_line"": ""Void PerformDivideByZero()"",
            ""lineno"": 10,
            ""colno"": 13,
            ""in_app"": false,
            ""post_context"": null
          }
        ]
      }
    }
  ]
}";

            JObject jPacket = JObject.Parse(json);
            JsonSchema schema = SchemaHelper.GetSchema();

            // This should output something like "Required properties are missing from object: level."
            jPacket.Validate(schema, (s, e) => Console.WriteLine(e.Message));
            Console.WriteLine(jPacket);

            var valid = jPacket.IsValid(schema);

            Assert.That(valid, Is.False);
        }
    }
}