# 📊 benchly 

Generate plots for [BenchmarkDotNet](https://github.com/dotnet/BenchmarkDotNet) using [Plotly.NET](https://github.com/plotly/Plotly.NET/).

# Getting started

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

```
  // * Export *
  BenchmarkDotNet.Artifacts\results\Benchly.Benchmarks.Md5VsSha256-report.csv
  BenchmarkDotNet.Artifacts\results\Benchly.Benchmarks.Md5VsSha256-report-github.md
  BenchmarkDotNet.Artifacts\results\Benchly.Benchmarks.Md5VsSha256-report.html
  BenchmarkDotNet.Artifacts\results\Benchly.Benchmarks.Md5VsSha256-boxplot.svg
  BenchmarkDotNet.Artifacts\results\Benchly.Benchmarks.Md5VsSha256-columnchart.svg
```