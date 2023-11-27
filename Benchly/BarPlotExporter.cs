using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using Plotly.NET.ImageExport;
using BenchmarkDotNet.Loggers;
using Plotly.NET;
using System;
using BenchmarkDotNet.Jobs;
using System.Xml.Linq;

namespace Benchly
{
    internal class BarPlotExporter : IExporter
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
            var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-barplot");

            var charts = new List<GenericChart.GenericChart>();

            var colors = GetColors(summary);

            var jobs = summary.Reports.Select(r => new 
            { 
                job = r.BenchmarkCase.Job.ResolvedId, 
                name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo,
                mean = r.Success ? ConvertNanosToMs(r.ResultStatistics.Mean) : 0
            }).GroupBy(r => r.job);

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

        private Dictionary<string, Color> GetColors(Summary summary) 
        {
            var jobs = summary.Reports.Select(r => r.BenchmarkCase.Job.ResolvedId).Distinct().ToList();

            var colorMap = new Dictionary<string, Color>();

            List<ColorKeyword> colorList = new List<ColorKeyword>() { ColorKeyword.FireBrick, ColorKeyword.IndianRed, ColorKeyword.Salmon };
            for (int i = 0; i < jobs.Count; i++)
            {
                colorMap.Add(jobs[i], Color.fromKeyword(colorList[i % 3]));
            }

            return colorMap;
        }
    }
}
