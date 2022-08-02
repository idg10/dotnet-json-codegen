using BenchmarkDotNet.Attributes;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCodeGen.Benchmarks
{
    public class FindElementBenchmarks : JsonBenchmarkBase
    {
        [Benchmark]
        public string FindNewtonsoftWholeArray()
        {
            this.jsonData.Position = 0;
            var data = Newtonsoft.Json.JsonConvert.DeserializeObject<PersonSerializable[]>(new StreamReader(this.jsonData, Encoding.UTF8).ReadToEnd())!;
            return data.FirstOrDefault(d => d.Name.GivenName == "Arthur9999")?.DateOfBirth ?? "";
        }
    }
}
