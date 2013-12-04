using System;
using System.IO;
using System.Linq;

using Newtonsoft.Json.Schema;

namespace SharpRaven.UnitTests
{
    public static class SchemaHelper
    {
        public static string GetSchemaPath()
        {
            var directory = new DirectoryInfo(Environment.CurrentDirectory);
            FileInfo file = null;

            while (directory != null && directory.Exists &&
                   (file = directory.EnumerateFiles("*.json")
                                    .FirstOrDefault(f => f.FullName.EndsWith("schema.json"))) == null)
            {
                directory = directory.Parent;
            }

            return file != null ? file.FullName : null;
        }


        public static JsonSchema GetSchema()
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
    }
}