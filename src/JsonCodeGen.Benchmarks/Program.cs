using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Running;

using JsonCodeGen.Benchmarks;

#if RELEASE
BenchmarkRunner.Run<FindElementBenchmarks>(DefaultConfig.Instance.AddJob(BenchmarkDotNet.Jobs.Job.Default.WithArguments(new[] { new MsBuildArgument("/p:EnablePreviewFeatures=true") })));
#else
BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args, new DebugInProcessConfig());
#endif