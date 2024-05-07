using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Engines;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using JsonCodeGen.Benchmarks;


#if RELEASE
bool tradeAccuracyForSpeed = args.Contains("tradeaccuracyforspeed");
Job job = Job.Default
    .WithArguments(new[] { new MsBuildArgument("/p:EnablePreviewFeatures=true") });
if (tradeAccuracyForSpeed)
{
    // Default max relative error is 0.02, and on my SB3, it takes 7:30 to run
    // all benchmarks with that setting.
    // Allowing 0.05 drops us to 5:42.
    // Allowing 0.1 drops us to 4:47.
    job = job.WithMaxRelativeError(0.05);
}
            
BenchmarkRunner.Run<FindElementBenchmarks>(DefaultConfig.Instance.AddJob(job));
#else
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
#endif