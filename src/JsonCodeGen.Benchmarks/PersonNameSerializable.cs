namespace JsonCodeGen.Benchmarks
{
    public class PersonNameSerializable
    {
        public PersonNameSerializable(string familyName)
        {
            FamilyName = familyName;
        }

        public string? GivenName { get; set; }
        public string FamilyName { get; set; }
        public IList<string> OtherNames { get; set; } = new List<string>();
    }
}