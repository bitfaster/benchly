using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Columns;
using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Environments;
using BenchmarkDotNet.Jobs;
using BenchmarkDotNet.Reports;
using BenchmarkDotNet.Running;
using BenchmarkDotNet.Validators;
using System.Collections.Immutable;
using System.Globalization;
using System.Security.Cryptography;

namespace Benchly.UnitTests
{
    public class TestBenchmarkRunner
    {
        /// <summary>
        /// Gets an empty Summary.
        /// </summary>
        public Summary EmptySummary => new Summary(string.Empty, new List<BenchmarkReport>().ToImmutableArray(),
                 BenchmarkDotNet.Environments.HostEnvironmentInfo.GetCurrent(),
                 string.Empty, string.Empty, TimeSpan.Zero, CultureInfo.CurrentCulture,
                 new List<ValidationError>().ToImmutableArray(),
                 new List<IColumnHidingRule>().ToImmutableArray());

        /// <summary>
        /// Runs a test benchmark.
        /// </summary>
        /// <returns>The summary result of the benchmark.</returns>
        public Summary GetSummary<TBenchmark>()
        {
            return BenchmarkRunner.Run(typeof(TBenchmark), DefaultConfig.Instance.WithOptions(ConfigOptions.DisableOptimizationsValidator));
        }
    }

    [MemoryDiagnoser]
    [DryJob(RuntimeMoniker.Net60)]
    [DryJob(RuntimeMoniker.Net48)]
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

    [MemoryDiagnoser, SimpleJob(RuntimeMoniker.Net60), SimpleJob(RuntimeMoniker.Net48)]
    [DryJob(RuntimeMoniker.Net60)]
    [DryJob(RuntimeMoniker.Net48)]
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
