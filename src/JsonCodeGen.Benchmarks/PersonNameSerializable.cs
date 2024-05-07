namespace JsonCodeGen.Benchmarks;

public class PersonNameSerializable(string familyName)
{
    public string? GivenName { get; set; }
    public string FamilyName { get; set; } = familyName;
    public IList<string> OtherNames { get; set; } = [];
}