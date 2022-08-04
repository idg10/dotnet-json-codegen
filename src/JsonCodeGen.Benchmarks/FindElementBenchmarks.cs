using BenchmarkDotNet.Attributes;

using Corvus.Json;

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCodeGen.Benchmarks
{
    [MemoryDiagnoser]
    public class FindElementBenchmarks : JsonBenchmarkBase
    {
        private const string LookingFor = "Arthur5000";
        private static byte[] LookingForUtf8 = Encoding.UTF8.GetBytes(LookingFor);
        private Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

        [Benchmark]
        public string FindWholeArrayNewtonsoftDeserialize()
        {
            string json = new StreamReader(new MemoryStream(this.jsonUtf8), Encoding.UTF8).ReadToEnd();
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<PersonSerializable[]>(json)!;
            var match = data.First(d => d.Name.GivenName == "Arthur5000");
            return match.DateOfBirth!;
        }

        [Benchmark]
        public string FindPerElementNewtonsoftDeserialize()
        {
            using TextReader tr = new StreamReader(new MemoryStream(this.jsonUtf8), Encoding.UTF8);
            using Newtonsoft.Json.JsonTextReader jr = new(tr);
            
            jr.Read();
            if (jr.TokenType != Newtonsoft.Json.JsonToken.StartArray)
            {
                throw new InvalidOperationException("Expected JSON array");
            }

            while (jr.Read() && jr.TokenType != Newtonsoft.Json.JsonToken.EndArray)
            {
                var data = serializer.Deserialize<PersonSerializable>(jr);
                if (data?.Name.GivenName == "Arthur5000")
                {
                    return data.DateOfBirth ?? "";
                }
            }

            return "";
        }

        [Benchmark]
        public string FindWholeArraySystemTextJsonDeserialize()
        {
            var data = System.Text.Json.JsonSerializer.Deserialize<PersonSerializable[]>(this.jsonUtf8, systemTextJsonSerializerOptions)!;
            var match = data.First(d => d.Name.GivenName == "Arthur5000");
            return match.DateOfBirth!;
        }

        [Benchmark]
        public string FindPerElementSystemTextJsonDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            foreach (System.Text.Json.JsonElement element in doc.RootElement.EnumerateArray())
            {
                var data = System.Text.Json.JsonSerializer.Deserialize<PersonSerializable>(element, systemTextJsonSerializerOptions)!;
                if (data.Name.GivenName == "Arthur5000")
                {
                    return data.DateOfBirth ?? "";
                }
            }

            return "";
        }

        [Benchmark]
        public string FindWholeArrayLinqSchemaGenDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            var data = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            var match = data.EnumerateArray().First(d => d.Name.GivenName == "Arthur5000");
            return match.DateOfBirth!;
        }

        [Benchmark]
        public string FindWholeArrayLoopSchemaGenDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            var array = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            foreach (var data in array.EnumerateArray())
            {
                if (data.Name.GivenName.EqualsString("Arthur5000"))
                {
                    return data.DateOfBirth.AsOptionalString() ?? "";
                }
            }
            return "";
        }

        [Benchmark]
        public string FindWholeArrayLoopSchemaGenValidateDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            var array = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            array.Validate(new Corvus.Json.ValidationContext(), ValidationLevel.Basic);
            foreach (var data in array.EnumerateArray())
            {
                if (data.Name.GivenName.EqualsString("Arthur5000"))
                {
                    return data.DateOfBirth.AsOptionalString() ?? "";
                }
            }
            return "";
        }

        [Benchmark]
        public string FindPerElementSchemaGenDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            foreach (System.Text.Json.JsonElement element in doc.RootElement.EnumerateArray())
            {
                var data = GenFromJsonSchema.Person.FromJson(element);

                if (data.Name.GivenName.EqualsString("Arthur5000"))
                {
                    return data.DateOfBirth.AsOptionalString() ?? "";
                }
            }

            return "";
        }

        [Benchmark]
        public string FindPerElementSchemaGenValidateDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            foreach (System.Text.Json.JsonElement element in doc.RootElement.EnumerateArray())
            {
                var data = GenFromJsonSchema.Person.FromJson(element);
                data.Validate(new ValidationContext(), ValidationLevel.Basic);

                if (data.Name.GivenName.EqualsString("Arthur5000"))
                {
                    return data.DateOfBirth.AsOptionalString() ?? "";
                }
            }

            return "";
        }


        [Benchmark]
        public string FindWholeArrayLinqSchemaGenDeserializeUtf8Comp()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            var data = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            var match = data.EnumerateArray().First(d => d.Name.GivenName.EqualsUtf8Bytes(LookingForUtf8));
            return match.DateOfBirth!;
        }

        [Benchmark]
        public string FindWholeArrayLoopSchemaGenDeserializeUtf8Comp()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            var array = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            foreach (var data in array.EnumerateArray())
            {
                if (data.Name.GivenName.EqualsUtf8Bytes(LookingForUtf8))
                {
                    return data.DateOfBirth.AsOptionalString() ?? "";
                }
            }
            return "";
        }

        [Benchmark]
        public string FindPerElementSchemaGenDeserializeUtf8Comp()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            foreach (System.Text.Json.JsonElement element in doc.RootElement.EnumerateArray())
            {
                var data = GenFromJsonSchema.Person.FromJson(element);

                if (data.Name.GivenName.EqualsUtf8Bytes(LookingForUtf8))
                {
                    return data.DateOfBirth.AsOptionalString() ?? "";
                }
            }

            return "";
        }

    }
}
