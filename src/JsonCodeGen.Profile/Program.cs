// See https://aka.ms/new-console-template for more information
using JsonCodeGen.Benchmarks;

var b = new FindElementBenchmarks();

Console.WriteLine("Waiting...");
Thread.Sleep(3000);


for (int i = 0; i < 10; ++i)
{
    //Stream result = b.SystemTextJsonSerializeCodegen();
    //    string result = b.FindPerElementSchemaGenValidateDeserialize();
    if (i % 10 == 0)
    {
        //Console.WriteLine(result.Length);
    }
}

Console.WriteLine("Done, waiting...");
Thread.Sleep(3000);
