using BenchmarkDotNet.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace JsonCodeGen.Benchmarks
{
    public abstract class JsonBenchmarkBase
    {
        protected readonly byte[] jsonUtf8;
        protected readonly Newtonsoft.Json.JsonSerializerSettings newtonsoftSerializerSettings = new() { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() };
        protected readonly JsonSerializerOptions systemTextJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);

        // Doesn't work with inheritance?
        //[GlobalSetup]
        //public void BuildBigJsonArray()
        protected JsonBenchmarkBase()
        {
            MemoryStream jsonData = new();
            using (Utf8JsonWriter w = new Utf8JsonWriter(jsonData))
            {
                w.WriteStartArray();

                PersonSerializable p = new(new PersonNameSerializable("Pewty")
                {
                    GivenName = "Arthur"
                })
                {
                    //DateOfBirth = new DateOnly(1954, 2, 4).ToShortDateString()
                    // Testing to see if format="date" is validated (it isn't).
                    DateOfBirth = "NotADate" // new DateOnly(1954, 2, 4).ToShortDateString()
                };
                for (int i = 0; i < 10000; ++i)
                {
                    JsonSerializer.Serialize(w, p, systemTextJsonSerializerOptions);
                    p.Name.GivenName = "Arthur" + i;
                    p.Name.FamilyName = "Pewty" + i;
                }

                w.WriteEndArray();
            }

            this.jsonUtf8 = jsonData.ToArray();
        }
    }
}
