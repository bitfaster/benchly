# benchly 📈
Generate plots for [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) using [Plotly.NET](https://github.com/plotly/Plotly.NET/).

# Getting started
    
Benchly is installed from NuGet:

`dotnet add package Benchly`

## Annotate benchmarks

Add plot exporter attributes to your benchmark, similar to the built in exporters:

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

![Benchly UnitTests TestBenchmarkRunner Md5VsSha256-20231126-152206](https://github.com/bitfaster/benchly/assets/12851828/d50f6d64-4acd-4a6a-a72b-0a7dd0871a79)
![Benchly UnitTests TestBenchmarkRunner Md5VsSha256-20231126-152206boxplot](https://github.com/bitfaster/benchly/assets/12851828/3fe033ca-20c2-4303-bec7-9cad19322370)
