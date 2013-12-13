using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

using SharpRaven.Data;

namespace SharpRaven.Serialization
{
    /// <summary>
    /// Converts <see cref="ErrorLevel"/> to a <see cref="System.String"/>.
    /// </summary>
    public class ErrorLevelConverter : StringEnumConverter
    {
        /// <summary>
        /// Writes the JSON representation of the object.
        /// </summary>
        /// <param name="writer">The <see cref="T:Newtonsoft.Json.JsonWriter" /> to write to.</param>
        /// <param name="value">The value.</param>
        /// <param name="serializer">The calling serializer.</param>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!(value is ErrorLevel))
                base.WriteJson(writer, value, serializer);

            var level = value.ToString().ToLowerInvariant();
            writer.WriteValue(level);
        }
    }
}