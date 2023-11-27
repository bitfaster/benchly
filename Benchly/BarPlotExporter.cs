using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using Plotly.NET.ImageExport;
using BenchmarkDotNet.Loggers;
using Plotly.NET;

namespace Benchly
{
    internal class BarPlotExporter : IExporter
    {
        public BarPlotExporter()
        {
        }

        public PlotInfo Info { get; set; } = new PlotInfo();

        public string Name => nameof(BarPlotExporter);

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            var title = this.Info.Title ?? summary.Title;
            var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-barplot");

            var charts = new List<GenericChart.GenericChart>();

            var colors = ColorMap.GetJobColors(summary, this.Info);

            var jobs = summary.Reports.Select(r => new 
            { 
                job = r.BenchmarkCase.Job.ResolvedId, 
                name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo,
                mean = r.Success ? ConvertNanosToMs(r.ResultStatistics.Mean) : 0
            }).GroupBy(r => r.job);

            // For this to group, we must invoke Chart2D.Chart.Column once per group
            foreach (var job in jobs)
            {
                var chart2 = Chart2D.Chart.Column<double, string, string, double, double>(job.Select(j => j.mean), job.Select(j => j.name).ToArray(), Name: job.Key, MarkerColor: colors[job.Key]);
                charts.Add(chart2);
            }

            var chart = Chart.Combine(charts)
                .WithAxisTitles("Time (ms)")
                .WithoutVerticalGridlines()
                .WithLayout(title);

            chart.SaveSVG(file, Width: 1000, Height: 600);

            return new[] { file + ".svg" };
        }

        // internal measurements are in nanos
        // https://github.com/dotnet/BenchmarkDotNet/blob/e4d37d03c0b1ef14e7bde224970bd0fc547fd95a/src/BenchmarkDotNet/Templates/BuildPlots.R#L63-L75
        private static double ConvertNanosToMs(double nanos)
        {
            return nanos * 0.000001;
        }

        public void ExportToLog(Summary summary, ILogger logger)
        {
            logger.WriteLine("Generated BarPlot");
        }
    }
}
