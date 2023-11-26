![Benchly UnitTests TestBenchmarkRunner Md5VsSha256-20231126-152206](https://github.com/bitfaster/benchly/assets/12851828/d50f6d64-4acd-4a6a-a72b-0a7dd0871a79)# benchly 📈
Generate plots for BenchmarkDotNet.

# Getting started
    
BitFaster.Caching is installed from NuGet:

`dotnet add package Benchly`

## Annotate benchmarks

```cs
    [BarPlot]
    [BoxPlot]
    public class MyBenchmark
    {
        [Benchmark]
        public void Bench()
        {
           // ...
        }
    }
```

## Plots generated in results directory

