using System.Text.Json;

namespace JsonCodeGen.Benchmarks
{
    public abstract class JsonBenchmarkBase
    {
        protected readonly byte[] jsonUtf8;
        protected readonly byte[] preallocatedOutputBuffer;
        protected readonly PersonSerializable[] people = new PersonSerializable[10000];
        protected readonly Newtonsoft.Json.JsonSerializerSettings newtonsoftSerializerSettings = new() { ContractResolver = new Newtonsoft.Json.Serialization.CamelCasePropertyNamesContractResolver() };
        protected readonly JsonSerializerOptions systemTextJsonSerializerOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
        protected readonly TestSerializationContext context;

        protected JsonBenchmarkBase()
        {
            context = new TestSerializationContext(systemTextJsonSerializerOptions);
            MemoryStream jsonData = new();
            using (Utf8JsonWriter w = new Utf8JsonWriter(jsonData))
            {
                w.WriteStartArray();

                for (int i = 0; i < people.Length; ++i)
                {
                    PersonSerializable p = new(new PersonNameSerializable("Pewty")
                    {
                        GivenName = "Arthur"
                    });
                    p.Name.GivenName = "Arthur" + i;
                    p.Name.FamilyName = "Pewty" + i;
                    p.DateOfBirth = new DateOnly(1954, 2, 4).AddDays(i).ToString("yyyy-MM-dd");

                    // Use one of these to see the effect of format="date" validation failures.
                    // Obviously wrong:
                    // p.DateOfBirth = "NotADate";
                    // Less obviously wrong: (produces "1954/2/4" but JSON schema dates are like "1954-02-04")
                    // p.DateOfBirth = new DateOnly(1954, 2, 4).ToShortDateString();

                    people[i] = p;
                    JsonSerializer.Serialize(w, p, systemTextJsonSerializerOptions);
                }

                w.WriteEndArray();
            }

            this.jsonUtf8 = jsonData.ToArray();
            this.preallocatedOutputBuffer = new byte[this.jsonUtf8.Length * 2];
        }
    }
}
