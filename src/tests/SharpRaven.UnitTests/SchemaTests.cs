#region License

// Copyright (c) 2013 The Sentry Team and individual contributors.
// All rights reserved.
// 
// Redistribution and use in source and binary forms, with or without modification, are permitted
// provided that the following conditions are met:
// 
//     1. Redistributions of source code must retain the above copyright notice, this list of
//        conditions and the following disclaimer.
// 
//     2. Redistributions in binary form must reproduce the above copyright notice, this list of
//        conditions and the following disclaimer in the documentation and/or other materials
//        provided with the distribution.
// 
//     3. Neither the name of the Sentry nor the names of its contributors may be used to
//        endorse or promote products derived from this software without specific prior written
//        permission.
// 
// THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR
// IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND
// FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR
// CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL
// DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE,
// DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY,
// WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN
// ANY WAY OUT OF THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

#endregion

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
            var schema = SchemaHelper.GenerateSchema(typeof(JsonPacket));
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