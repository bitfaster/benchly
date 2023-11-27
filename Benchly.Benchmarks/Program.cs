using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Running;
using Benchly;

BenchmarkSwitcher.FromAssembly(typeof(Program).Assembly).Run(args);
