#define System_Text_Json
#define System_Text_Json_Codegen
#define CustomCodeGen
#define TestValidation

using BenchmarkDotNet.Attributes;

using Corvus.Json;

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
            var ms = new MemoryStream(this.jsonUtf8.Length);
            System.Text.Json.JsonSerializer.Serialize(ms, this.people, systemTextJsonSerializerOptions);
            return ms;
        }

        [Benchmark]
        public Stream SystemTextJsonSerializeCodegen()
        {
            var ms = new MemoryStream(this.jsonUtf8.Length);
            using (Utf8JsonWriter jw = new(ms))
            {
                jw.WriteStartArray();
                foreach (PersonSerializable p in this.people)
                {
                    TestSerializationContext.Default.PersonSerializable.SerializeHandler!(jw, p);
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
        public string FindWholeArrayLoopSchemaGenValidateDeserialize()
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
        public string FindPerElementSchemaGenValidateDeserialize()
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
#endif

#endif
    }
}
