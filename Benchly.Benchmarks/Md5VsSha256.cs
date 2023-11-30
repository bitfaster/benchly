using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Security.Cryptography;

namespace Benchly.Benchmarks
{
    [BoxPlot(Title = "Box Plot", Colors = "skyblue,slateblue", Height = 800)]
    [ColumnChart(Title = "Column Chart ({JOB})", Colors = "skyblue,slateblue", Height =800, Output = OutputMode.PerJob)]
    [Histogram(Width=500)]
    [Timeline(Width = 500)]
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
}
