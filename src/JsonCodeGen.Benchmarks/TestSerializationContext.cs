using System.Text.Json.Serialization;

namespace JsonCodeGen.Benchmarks
{
    [JsonSerializable(typeof(PersonSerializable))]
    [JsonSerializable(typeof(PersonSerializable[]))]
    [JsonSerializable(typeof(PersonNameSerializable))]
    public partial class TestSerializationContext : JsonSerializerContext
    {
    }
}
