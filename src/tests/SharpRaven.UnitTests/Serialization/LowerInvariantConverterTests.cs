using Newtonsoft.Json;

using NSubstitute;

using NUnit.Framework;

using SharpRaven.Serialization;

namespace SharpRaven.UnitTests.Serialization {
    [TestFixture]
    public class LowerInvariantConverterTests {
        [TestCase(EnumTestable.One, "one")]
        [TestCase(EnumTestable.Two, "two")]
        [TestCase(EnumTestable.ThreE, "three")]
        public void Should_Convert_Enum_To_Lowercase(EnumTestable enumTestable, string result)
        {
            var lowerInvariantConverter = new LowerInvariantConverter();

            var jsonWriter = Substitute.For<JsonWriter>();
            
            lowerInvariantConverter.WriteJson(jsonWriter, enumTestable, Substitute.For<JsonSerializer>());

            jsonWriter.Received().WriteValue(result);
        }

        [Test]
        public void Should_Not_Call_Write_Null_value()
        {
            var lowerInvariantConverter = new LowerInvariantConverter();

            var jsonWriter = Substitute.For<JsonWriter>();
            
            lowerInvariantConverter.WriteJson(jsonWriter, null, Substitute.For<JsonSerializer>());

            jsonWriter.DidNotReceiveWithAnyArgs().WriteValue("");
        }

        [Test]
        public void Should_Not_Call_Write_If_Not_Enum_Object()
        {
            var lowerInvariantConverter = new LowerInvariantConverter();

            var jsonWriter = Substitute.For<JsonWriter>();
            
            lowerInvariantConverter.WriteJson(jsonWriter, "any string", Substitute.For<JsonSerializer>());

            jsonWriter.DidNotReceiveWithAnyArgs().WriteValue("");
        }


        public enum EnumTestable
        {
            One, Two, ThreE
        }
    }
}
