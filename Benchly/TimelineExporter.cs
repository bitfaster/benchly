using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Plotly.NET;
using Plotly.NET.ImageExport;

namespace Benchly
{
    internal class TimelineExporter : IExporter
    {
        public PlotInfo Info { get; set; } = new PlotInfo();

        public string Name => nameof(TimelineExporter);

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            string baseName = ExporterBase.GetFileName(summary);
            List<string> files = new List<string>();
            foreach (var r in summary.Reports) 
            {
                if (!r.Success) 
                {
                    continue;
                }

                var title = $"{r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo} ({r.BenchmarkCase.Job.ResolvedId})";
                var file = Path.Combine(summary.ResultsDirectoryPath, baseName + "-timeline-" + title);

                var data = r.AllMeasurements.Where(m => m.IterationMode == BenchmarkDotNet.Engines.IterationMode.Workload && m.IterationStage == BenchmarkDotNet.Engines.IterationStage.Actual).Select(m => m.GetAverageTime().Nanoseconds).ToArray();

                var iters = Enumerable.Range(0, data.Length);

                Chart2D.Chart.Line<int, double, string>(x: iters, y: data)
                    .WithoutVerticalGridlines()
                    .WithAxisTitles("Iteration", "Latency (ns)")
                    .WithLayout(title)
                    .SaveSVG(file, Width: 1000, Height: 600);

                files.Add(file + ".svg");
            }

            return files;
        }

        public void ExportToLog(Summary summary, ILogger logger)
        {
        }
    }
}
