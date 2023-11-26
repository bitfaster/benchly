using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using Plotly.NET.ImageExport;
using BenchmarkDotNet.Loggers;

using Chart = Plotly.NET.CSharp.Chart;

namespace Benchly
{
    public class BarPlotExporter : IExporter
    {
        private readonly PlotInfo plotInfo = new PlotInfo();

        public BarPlotExporter(PlotInfo plotInfo)
        {
            this.plotInfo = plotInfo;
        }

        public string Name => nameof(BarPlotExporter);

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            var title = this.plotInfo.Title ?? summary.Title;
            var file = Path.Combine(summary.ResultsDirectoryPath, summary.Title);

            var jobs = summary.Reports.Select(r => r.BenchmarkCase.Job.DisplayInfo);
            var names = summary.Reports.Select(r => r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo).ToArray();
            var mean = summary.Reports.Select(r => r.BuildResult.IsBuildSuccess ? ConvertNanosToMs(r.ResultStatistics.Mean) : 0);

            var x = summary.Reports.Select(r => r.AllMeasurements.Where(m => m.IterationMode == BenchmarkDotNet.Engines.IterationMode.Workload).Select(m => m.Nanoseconds));

            //var fn = $"{benchName} ({job.Key})";
            var chart = Chart.Column<double, string, string>(mean, names, MarkerColor: Plotly.NET.Color.fromKeyword(Plotly.NET.ColorKeyword.IndianRed))
                //.WithYErrorStyle<double, IConvertible>(stdDev)
                .WithAxisTitles("Time (ms)")
                .WithoutVerticalGridlines()
                .WithLayout(title);

            chart.SaveSVG(file, Width: 1000, Height: 600);

            return new[] { file };
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
