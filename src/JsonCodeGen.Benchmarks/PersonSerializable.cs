using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonCodeGen.Benchmarks
{
    public class PersonSerializable
    {
        public PersonNameSerializable Name { get; set; }

        public PersonSerializable(PersonNameSerializable name)
        {
            Name = name;
        }

        public string? DateOfBirth { get; set; }
    }
}
