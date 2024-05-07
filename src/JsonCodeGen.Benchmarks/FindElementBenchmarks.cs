#define NEWTONSOFT_JSON
#define System_Text_Json
#define System_Text_Json_Codegen
#define CustomCodeGen
//#define TestValidation

using BenchmarkDotNet.Attributes;

using Corvus.Json;

using System.Text;
using System.Text.Json;

namespace JsonCodeGen.Benchmarks;

// .NET 8.0
// |                                  Method |      Mean |     Error |    StdDev |      Gen0 |      Gen1 |     Gen2 |  Allocated |
// |---------------------------------------- |----------:|----------:|----------:|----------:|----------:|---------:|-----------:|
// |     FindWholeArrayNewtonsoftDeserialize | 41.906 ms | 1.3737 ms | 1.2850 ms | 3750.0000 | 1500.0000 | 500.0000 | 22666583 B |
// |     FindPerElementNewtonsoftDeserialize | 11.503 ms | 0.2334 ms | 0.2069 ms | 2234.3750 |         - |        - |  9359650 B |
// | FindWholeArraySystemTextJsonDeserialize | 29.465 ms | 0.9292 ms | 0.8692 ms |  812.5000 |  468.7500 | 156.2500 |  4335638 B |
// | FindPerElementSystemTextJsonDeserialize | 14.437 ms | 0.0718 ms | 0.0637 ms | 1000.0000 |         - |        - |  4192933 B |
// |           FindSystemTextJsonJsonElement |  4.965 ms | 0.0580 ms | 0.0514 ms |         - |         - |        - |      205 B |
// |            FindSystemTextUtf8JsonReader |  1.913 ms | 0.0224 ms | 0.0187 ms |         - |         - |        - |       49 B |

// .NET 7.0
// |                                  Method |      Mean |     Error |    StdDev |      Gen0 |      Gen1 |     Gen2 |  Allocated |
// |---------------------------------------- |----------:|----------:|----------:|----------:|----------:|---------:|-----------:|
// |     FindWholeArrayNewtonsoftDeserialize | 62.968 ms | 3.0703 ms | 3.0155 ms | 3625.0000 | 1375.0000 | 375.0000 | 22666560 B |
// |     FindPerElementNewtonsoftDeserialize | 19.336 ms | 0.9628 ms | 0.9456 ms | 2218.7500 |         - |        - |  9359666 B |
// | FindWholeArraySystemTextJsonDeserialize | 42.666 ms | 2.0232 ms | 1.7935 ms |  846.1538 |  538.4615 | 153.8462 |  4860081 B |
// | FindPerElementSystemTextJsonDeserialize | 19.740 ms | 0.6957 ms | 0.7444 ms | 1093.7500 |         - |        - |  4633062 B |
// |           FindSystemTextJsonJsonElement |  6.336 ms | 0.2820 ms | 0.2637 ms |         - |         - |        - |      222 B |
// |            FindSystemTextUtf8JsonReader |  2.347 ms | 0.1115 ms | 0.1095 ms |         - |         - |        - |       51 B |

[MemoryDiagnoser]
public class FindElementBenchmarks : JsonBenchmarkBase
{
    private const string LookingFor = "Arthur5000";
    private static readonly byte[] LookingForUtf8 = Encoding.UTF8.GetBytes(LookingFor);
    private readonly Newtonsoft.Json.JsonSerializer serializer = new();

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
#pragma warning disable IDE0066 // Convert switch statement to expression - pretty terrible advice in this case, frankly.
                        switch (reader.TokenType)
                        {
                            case JsonTokenType.StartObject:
                                depth += 1;
                                break;

                            default:
                                throw new InvalidOperationException($"Did not expect {reader.TokenType} as name value");
                        }
#pragma warning restore IDE0066 // Convert switch statement to expression
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
    // .NET 8.0
    // |                                  Method |      Mean |     Error |    StdDev |      Gen0 |      Gen1 |     Gen2 |  Allocated |
    // |---------------------------------------- |----------:|----------:|----------:|----------:|----------:|---------:|-----------:|
    // |       SystemTextJsonSerializeReflection |  5.079 ms | 0.0747 ms | 0.0699 ms |         - |         - |        - |      533 B |
    // |  SystemTextJsonSerializeMetadataCodeGen |  4.979 ms | 0.0585 ms | 0.0489 ms |         - |         - |        - |      533 B |
    // |    SystemTextJsonSerializeWriterCodegen |  2.406 ms | 0.0267 ms | 0.0208 ms |         - |         - |        - |     5105 B |

    // .NET 7.0
    // |                                  Method |      Mean |     Error |    StdDev |      Gen0 |      Gen1 |     Gen2 |  Allocated |
    // |---------------------------------------- |----------:|----------:|----------:|----------:|----------:|---------:|-----------:|
    // |       SystemTextJsonSerializeReflection |  7.558 ms | 0.3695 ms | 0.4255 ms |   93.7500 |         - |        - |   400541 B |
    // |  SystemTextJsonSerializeMetadataCodeGen |  6.725 ms | 0.3200 ms | 0.3286 ms |   93.7500 |         - |        - |   400540 B |
    // |    SystemTextJsonSerializeWriterCodegen |  3.237 ms | 0.1400 ms | 0.1310 ms |         - |         - |        - |     5107 B |
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
    // .NET 8.0
    // |     FindWholeArrayNewtonsoftDeserialize | 41.906 ms | 1.3737 ms | 1.2850 ms | 3750.0000 | 1500.0000 | 500.0000 | 22666583 B |
    // |     FindPerElementNewtonsoftDeserialize | 11.503 ms | 0.2334 ms | 0.2069 ms | 2234.3750 |         - |        - |  9359650 B |
    // | FindWholeArraySystemTextJsonDeserialize | 29.465 ms | 0.9292 ms | 0.8692 ms |  812.5000 |  468.7500 | 156.2500 |  4335638 B |
    // | FindPerElementSystemTextJsonDeserialize | 14.437 ms | 0.0718 ms | 0.0637 ms | 1000.0000 |         - |        - |  4192933 B |
    // |           FindSystemTextJsonJsonElement |  4.965 ms | 0.0580 ms | 0.0514 ms |         - |         - |        - |      205 B |
    // |            FindSystemTextUtf8JsonReader |  1.913 ms | 0.0224 ms | 0.0187 ms |         - |         - |        - |       49 B |
    // |  FindWholeArrayLinqSchemaGenDeserialize |  5.297 ms | 0.1327 ms | 0.1242 ms |         - |         - |        - |      333 B |
    // |  FindWholeArrayLoopSchemaGenDeserialize |  4.912 ms | 0.0930 ms | 0.0824 ms |         - |         - |        - |      125 B |
    // |      FindPerElementSchemaGenDeserialize |  4.902 ms | 0.0766 ms | 0.0716 ms |         - |         - |        - |      125 B |

    // .NET 7.0
    // |                                  Method |      Mean |     Error |    StdDev |      Gen0 |      Gen1 |     Gen2 |  Allocated |
    // |---------------------------------------- |----------:|----------:|----------:|----------:|----------:|---------:|-----------:|
    // |     FindWholeArrayNewtonsoftDeserialize | 61.979 ms | 2.8877 ms | 3.6520 ms | 3625.0000 | 1375.0000 | 375.0000 | 22666603 B |
    // |     FindPerElementNewtonsoftDeserialize | 19.823 ms | 0.9201 ms | 0.8607 ms | 2218.7500 |         - |        - |  9359686 B |
    // | FindWholeArraySystemTextJsonDeserialize | 42.555 ms | 2.1255 ms | 3.1155 ms |  846.1538 |  538.4615 | 153.8462 |  4860398 B |
    // | FindPerElementSystemTextJsonDeserialize | 20.076 ms | 0.9655 ms | 1.1119 ms | 1093.7500 |         - |        - |  4633062 B |
    // |           FindSystemTextJsonJsonElement |  6.230 ms | 0.2256 ms | 0.2000 ms |         - |         - |        - |      222 B |
    // |            FindSystemTextUtf8JsonReader |  2.441 ms | 0.1214 ms | 0.1535 ms |         - |         - |        - |       49 B |
    // |  FindWholeArrayLinqSchemaGenDeserialize |  6.729 ms | 0.3316 ms | 0.3257 ms |         - |         - |        - |      350 B |
    // |  FindWholeArrayLoopSchemaGenDeserialize |  6.360 ms | 0.3007 ms | 0.2813 ms |         - |         - |        - |      142 B |
    // |      FindPerElementSchemaGenDeserialize |  6.296 ms | 0.3063 ms | 0.3404 ms |         - |         - |        - |      142 B |

    [Benchmark]
    public string FindWholeArrayLinqSchemaGenDeserialize()
    {
        using System.Text.Json.JsonDocument doc = System.Text.Json.JsonDocument.Parse(this.jsonUtf8);
        GenFromJsonSchema.PersonArray data = GenFromJsonSchema.PersonArray.FromJson(doc.RootElement);
        GenFromJsonSchema.Person match = data.EnumerateArray().First(d => d.Name.GivenName == "Arthur5000");
        return (string)match.DateOfBirth;
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
                return (string)data.DateOfBirth;
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
                return (string)data.DateOfBirth;
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
