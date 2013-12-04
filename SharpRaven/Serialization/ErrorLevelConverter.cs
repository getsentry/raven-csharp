using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using SharpRaven.Data;

namespace SharpRaven.Serialization
{
    public class ErrorLevelConverter : StringEnumConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is ErrorLevel))
                base.WriteJson(writer, value, serializer);

            var level = value.ToString().ToLowerInvariant();
            writer.WriteValue(level);
        }
    }
}