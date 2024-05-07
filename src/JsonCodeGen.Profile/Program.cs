// See https://aka.ms/new-console-template for more information
using JsonCodeGen.Benchmarks;

#pragma warning disable IDE0059 // Unnecessary assignment of a value - code commented in and out as required, leading to spurious warning here.
var b = new FindElementBenchmarks();
#pragma warning restore IDE0059

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
