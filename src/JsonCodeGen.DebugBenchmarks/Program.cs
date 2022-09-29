// See https://aka.ms/new-console-template for more information

using JsonCodeGen.Benchmarks;

using NodaTime.Text;

var bm = new FindElementBenchmarks();

Console.WriteLine(LocalDatePattern.Iso.PatternText);

switch (args[0])
{
    //case "FindPerElementSchemaGenDeserialize":
    //    bm.FindPerElementSchemaGenDeserialize();
    //    break;

    //case "FindPerElementSchemaGenValidateDeserialize":
    //    bm.FindPerElementSchemaGenValidateDeserialize();
    //    break;

    //case "FindWholeArraySystemTextJsonElements":
    //    Console.WriteLine(bm.FindSystemTextJsonJsonElement());
    //    break;

    //case "FindSystemTextUtf8JsonReader":
    //    Console.WriteLine(bm.FindSystemTextUtf8JsonReader());
    //    break;

    case "SystemTextJsonSerializeReflection":
        Console.WriteLine(bm.SystemTextJsonSerializeReflection());
        break;
}