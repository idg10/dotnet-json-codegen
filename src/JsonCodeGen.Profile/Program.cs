// See https://aka.ms/new-console-template for more information
using JsonCodeGen.Benchmarks;

var b = new FindElementBenchmarks();

Console.WriteLine("Waiting...");
Thread.Sleep(3000);

for (int i = 0; i < 500; ++i)
{
    string result = b.FindWholeArrayLoopSchemaGenDeserialize();
    if (i % 10 == 0)
    {
        Console.WriteLine(result );
    }
}