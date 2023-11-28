using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Jobs;
using System.Security.Cryptography;

namespace Benchly.Benchmarks
{
    [BoxPlot(Title = "Box Plot")]
    [ColumnChart(Title = "Column Chart", Output=OutputMode.Combined)]
    [Histogram]
    [Timeline]
    [MemoryDiagnoser, SimpleJob(RuntimeMoniker.Net60), SimpleJob(RuntimeMoniker.Net48)]
    public class Md5VsSha256Params
    {
        private byte[] data;

        private readonly SHA256 sha256 = SHA256.Create();
        private readonly MD5 md5 = MD5.Create();

        [Params(128, 1024, 16384)]
        public int Size { get; set; }

        [GlobalSetup]
        public void Setup()
        {
            data = new byte[Size];
            new Random(42).NextBytes(data);
        }

        [Benchmark]
        public byte[] Sha256() => sha256.ComputeHash(data);

        [Benchmark]
        public byte[] Md5() => md5.ComputeHash(data);
    }
}
