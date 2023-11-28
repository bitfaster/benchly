# 📊 benchly 

Use Benchly to automatically export graphical [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) results without installing additional tools such as R. Benchly runs seamlessly as part of benchmark execution and is compatible with GitHub actions. Benchly produces high quality charts using [Plotly.NET](https://github.com/plotly/Plotly.NET/).

[![NuGet version](https://badge.fury.io/nu/Benchly.svg)](https://badge.fury.io/nu/benchly) ![Nuget](https://img.shields.io/nuget/dt/benchly) 

Benchly supports 4 different plots:

- Column chart: shows the relative latency of results.
- Box plot: shows the relative variability of results.
- Histogram: shows distribution of results.
- Timeline: shows the latency trend through time.

# Getting started
    
Benchly is installed from NuGet:

`dotnet add package Benchly`

## Annotate benchmarks

Add plot exporter attributes to your benchmark:

```cs
    [BoxPlot(Title = "Box Plot", Colors = "skyblue,slateblue")]
    [ColumnChart(Title = "Column Chart", Colors = "skyblue,slateblue")]
    [MemoryDiagnoser, SimpleJob(RuntimeMoniker.Net60), SimpleJob(RuntimeMoniker.Net48)]
    public class Md5VsSha256
    {
        private const int N = 10000;
        private readonly byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        public Md5VsSha256()
        {
            data = new byte[N];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);
    }
```

Plots are written to the results directory after running the benchmarks, like the built in exporters:

![image](https://github.com/bitfaster/benchly/assets/12851828/7628b105-f367-4be2-8032-ee4f318b4e85)

![Benchly Benchmarks Md5VsSha256-boxplot](https://github.com/bitfaster/benchly/assets/12851828/f906002c-57cb-4d82-9fca-266160efa5e9)
![Benchly Benchmarks Md5VsSha256-barplot](https://github.com/bitfaster/benchly/assets/12851828/c9b2abe3-c9a3-4bfa-8678-7fe11dca468a)

# Under the hood

Benchly uses Plotly.NET. Ironically for a performance measurement tool, whilst convenient this is not a performant approach to generating plots. Internally, FSharp invokes plotly.js running inside a headless instance of chromium managed by pupetteer. The first time benchly runs, chromium will be downloaded into the bin directory causing short delay.

# Credits

Based on this repo that shows how to export benchmark data to Excel:
https://github.com/CodeTherapist/BenchmarkDotNetXlsxExporter
