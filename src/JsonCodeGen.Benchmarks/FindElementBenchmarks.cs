#define NEWTONSOFT_JSON
#define System_Text_Json
#define System_Text_Json_Codegen
#define CustomCodeGen
//#define TestValidation

using BenchmarkDotNet.Attributes;

using Corvus.Json;

using Microsoft.CodeAnalysis.CSharp.Syntax;
using Microsoft.Diagnostics.Tracing.Parsers.Clr;

using System.Text;
using System.Text.Json;

namespace JsonCodeGen.Benchmarks
{
    [MemoryDiagnoser]
    public class FindElementBenchmarks : JsonBenchmarkBase
    {
        private const string LookingFor = "Arthur5000";
        private static byte[] LookingForUtf8 = Encoding.UTF8.GetBytes(LookingFor);
        private Newtonsoft.Json.JsonSerializer serializer = new Newtonsoft.Json.JsonSerializer();

#if NEWTONSOFT_JSON
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

        // You might think this would be more efficient than reading the entire string
        // into a single string up front. In practice, the speed is almost identical,
        // and the GC patterns are slightly different - this seems to do fewer Gen0
        // but more Gen1 and Gen2 collects. It does allocate about 20% less memory
        // though.
        ////[Benchmark]
        ////public string FindWholeArrayNewtonsoftDeserializeViaStreamReader()
        ////{
        ////    using StreamReader json = new(new MemoryStream(this.jsonUtf8), Encoding.UTF8);
        ////    using Newtonsoft.Json.JsonTextReader rdr = new(json);
        ////    var data = this.serializer.Deserialize<PersonSerializable[]>(rdr)!;
        ////    var match = data.First(d => d.Name.GivenName == "Arthur5000");
        ////    return match.DateOfBirth!;
        ////}
#endif

#if System_Text_Json
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
        public string FindSystemTextJsonJsonElement()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            System.Text.Json.JsonElement match = doc.RootElement
                .EnumerateArray()
                .First(d => d.TryGetProperty("name", out JsonElement namePropertyValue)
                    && namePropertyValue.TryGetProperty("givenName", out JsonElement givenNamePropertyValue)
                    && givenNamePropertyValue.ValueEquals("Arthur5000"));
            return match.GetProperty("dateOfBirth").GetString()!;
        }

        [Benchmark]
        public string FindSystemTextUtf8JsonReader()
        {
            var reader = new Utf8JsonReader(this.jsonUtf8.AsSpan());

            bool inArray = false;
            bool inName = false;
            bool inDateOfBirth = false;
            bool inGivenName = false;
            bool givenNameIsMatch = false;
            bool givenNameIsNonMatch = false;
            Span<byte> dateOfBirthBuffer = stackalloc byte[10];
            int dateOfBirthLength = 0;
            bool seenDateOfBirthForThisItem = false;
            int depth = 0;
            while (reader.Read())
            {
                if (!inArray)
                {
                    if (reader.TokenType == JsonTokenType.StartArray)
                    {
                        inArray = true;
                    }
                }
                else
                {
                    if (depth == 0)
                    {
                        bool endOfArray = false;
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.StartObject:
                                depth++;
                                seenDateOfBirthForThisItem = false;
                                givenNameIsNonMatch = false;
                                break;

                            case JsonTokenType.EndArray:
                                endOfArray = true;
                                break;

                            default:
                                throw new InvalidOperationException($"Did not expect {reader.TokenType} at depth 0");
                        }
                        if (endOfArray)
                        {
                            break;
                        }
                    }
                    else if (depth == 1)
                    {
                        if (givenNameIsNonMatch)
                        {
                            // No point looking at the rest of the object
                            if (reader.TokenType == JsonTokenType.EndObject)
                            {
                                depth -= 1;
                            }
                            else
                            {
                                reader.Skip();
                            }
                            continue;
                        }

                        if (inName)
                        {
                            switch (reader.TokenType)
                            {
                                case JsonTokenType.StartObject:
                                    depth += 1;
                                    break;

                                default:
                                    throw new InvalidOperationException($"Did not expect {reader.TokenType} as name value");
                            }
                        }
                        else if (inDateOfBirth)
                        {
                            switch (reader.TokenType)
                            {
                                case JsonTokenType.String:
                                    if (givenNameIsMatch)
                                    {
                                        return reader.GetString()!;
                                    }
                                    reader.ValueSpan[1..^2].CopyTo(dateOfBirthBuffer);
                                    dateOfBirthLength = reader.ValueSpan.Length - 2;
                                    inDateOfBirth = false;
                                    break;

                                default:
                                    throw new InvalidOperationException($"Did not expect {reader.TokenType} as dateOfBirth value");
                            }
                        }
                        else
                        {
                            switch (reader.TokenType)
                            {
                                case JsonTokenType.PropertyName:
                                    if (reader.ValueTextEquals("name"))
                                    {
                                        inName = true;
                                    }
                                    else if (reader.ValueTextEquals("dateOfBirth"))
                                    {
                                        inDateOfBirth = true;
                                    }
                                    else
                                    {
                                        reader.Skip();
                                    }
                                    break;

                                default:
                                    throw new InvalidOperationException($"Did not expect {reader.TokenType} at depth 0");
                            }
                        }
                    }
                    else
                    {
                        // We're in the name if we get here.
                        if (inGivenName)
                        {
                            switch (reader.TokenType)
                            {
                                case JsonTokenType.String:
                                    givenNameIsMatch = reader.ValueTextEquals("Arthur5000");
                                    if (givenNameIsMatch)
                                    {
                                        if (seenDateOfBirthForThisItem)
                                        {
                                            return Encoding.UTF8.GetString(dateOfBirthBuffer[..dateOfBirthLength]);
                                        }
                                    }
                                    else
                                    {
                                        givenNameIsNonMatch = true;
                                    }
                                    inGivenName = false;
                                    break;

                                default:
                                    throw new InvalidOperationException($"Did not expect {reader.TokenType} as dateOfBirth value");
                            }
                        }
                        else
                        {
                            switch (reader.TokenType)
                            {
                                case JsonTokenType.PropertyName:
                                    if (reader.ValueTextEquals("givenName"))
                                    {
                                        inGivenName = true;
                                    }
                                    else
                                    {
                                        reader.Skip();
                                    }
                                    break;

                                case JsonTokenType.EndObject:
                                    depth -= 1;
                                    inName = false;
                                    break;

                                default:
                                    throw new InvalidOperationException($"Did not expect {reader.TokenType} at depth 0");
                            }
                        }
                    }
                }
            }
            throw new InvalidOperationException("Didn't find match");
        }
#endif

#if System_Text_Json_Codegen

        [Benchmark]
        public Stream SystemTextJsonSerializeReflection()
        {
            var ms = new MemoryStream(this.preallocatedOutputBuffer, writable: true);
            System.Text.Json.JsonSerializer.Serialize(ms, this.people, systemTextJsonSerializerOptions);
            return ms;
        }

        [Benchmark]
        public Stream SystemTextJsonSerializeMetadataCodeGen()
        {
            GC.KeepAlive( context.PersonSerializable);
            var ms = new MemoryStream(this.preallocatedOutputBuffer, writable: true);
            System.Text.Json.JsonSerializer.Serialize(ms, this.people, context.PersonSerializableArray);
            return ms;
        }

        [Benchmark]
        public Stream SystemTextJsonSerializeWriterCodegen()
        {
            var ms = new MemoryStream(this.preallocatedOutputBuffer, writable: true);
            using (Utf8JsonWriter jw = new(ms))
            {
                jw.WriteStartArray();
                int i = 0;
                foreach (PersonSerializable p in this.people)
                {
                    TestSerializationContext.Default.PersonSerializable.SerializeHandler!(jw, p);
                    if (++i == 10)
                    {
                        jw.Flush();
                        i = 0;
                    }
                }
                jw.WriteEndArray();
            }
            return ms;
        }
#endif

#if CustomCodeGen
        [Benchmark]
        public string FindWholeArrayLinqSchemaGenDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            GenFromJsonSchema.PersonArray data = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            GenFromJsonSchema.Person match = data.EnumerateArray().First(d => d.Name.GivenName == "Arthur5000");
            return match.DateOfBirth!;
        }

        [Benchmark]
        public string FindWholeArrayLoopSchemaGenDeserialize()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            GenFromJsonSchema.PersonArray array = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            foreach (GenFromJsonSchema.Person data in array.EnumerateArray())
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

        //[Benchmark]
        //public string FindWholeArrayLinqSchemaGenDeserializeUtf8Comp()
        //{
        //    using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
        //    var data = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
        //    var match = data.EnumerateArray().First(d => d.Name.GivenName.EqualsUtf8Bytes(LookingForUtf8));
        //    return match.DateOfBirth!;
        //}

        //[Benchmark]
        //public string FindWholeArrayLoopSchemaGenDeserializeUtf8Comp()
        //{
        //    using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
        //    var array = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
        //    foreach (var data in array.EnumerateArray())
        //    {
        //        if (data.Name.GivenName.EqualsUtf8Bytes(LookingForUtf8))
        //        {
        //            return data.DateOfBirth.AsOptionalString() ?? "";
        //        }
        //    }
        //    return "";
        //}

        //[Benchmark]
        //public string FindPerElementSchemaGenDeserializeUtf8Comp()
        //{
        //    using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
        //    foreach (System.Text.Json.JsonElement element in doc.RootElement.EnumerateArray())
        //    {
        //        var data = GenFromJsonSchema.Person.FromJson(element);

        //        if (data.Name.GivenName.EqualsUtf8Bytes(LookingForUtf8))
        //        {
        //            return data.DateOfBirth.AsOptionalString() ?? "";
        //        }
        //    }

        //    return "";
        //}

#if TestValidation
        [Benchmark]
        public string FindWholeArrayLoopSchemaGenValidateIndividual()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            var array = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            if (array.IsValid())
            {
                foreach (var data in array.EnumerateArray())
                {
                    if (data.Name.GivenName.EqualsString("Arthur5000"))
                    {
                        return data.DateOfBirth.AsOptionalString() ?? "";
                    }
                }
            }
            return "";
        }

        [Benchmark]
        public string FindPerElementSchemaGenValidateIndividual()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            foreach (System.Text.Json.JsonElement element in doc.RootElement.EnumerateArray())
            {
                var data = GenFromJsonSchema.Person.FromJson(element);
                if (data.IsValid())
                {
                    if (data.Name.GivenName.EqualsString("Arthur5000"))
                    {
                        return data.DateOfBirth.AsOptionalString() ?? "";
                    }
                }
            }

            return "";
        }

        [Benchmark]
        public string FindWholeArrayLoopSchemaGenValidateAll()
        {
            using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
            var array = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
            foreach (var data in array.EnumerateArray())
            {
                if (data.IsValid())
                {
                    if (data.Name.GivenName.EqualsString("Arthur5000"))
                    {
                        return data.DateOfBirth.AsOptionalString() ?? "";
                    }
                }
            }
            return "";
        }
#endif

#endif
    }
}
