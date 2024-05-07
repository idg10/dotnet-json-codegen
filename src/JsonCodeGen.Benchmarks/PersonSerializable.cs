namespace JsonCodeGen.Benchmarks;

public class PersonSerializable(PersonNameSerializable name)
{
    public PersonNameSerializable Name { get; set; } = name;

    public string? DateOfBirth { get; set; }
}
