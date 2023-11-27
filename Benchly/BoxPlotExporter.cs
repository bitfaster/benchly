using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;
using Plotly.NET;
using Plotly.NET.ImageExport;

namespace Benchly
{
    // Based on https://github.com/CodeTherapist/BenchmarkDotNetXlsxExporter
    // https://pvk.ca/Blog/2012/07/03/binary-search-star-eliminates-star-branch-mispredictions/
    internal class BoxPlotExporter : IExporter
    {
        private readonly PlotInfo plotInfo = new PlotInfo();

        public BoxPlotExporter(PlotInfo plotInfo) 
        {
            this.plotInfo = plotInfo;
        }

        public string Name => nameof(BoxPlotExporter);

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            var title = this.plotInfo.Title ?? summary.Title;
            var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-boxplot");

            var plots = summary.Reports.Select(r => new BoxPlotInfo(r)).ToList();
            string timeUnit = ConvertTime(plots);

            var jobs = plots.GroupBy(p => p.Job);

            // https://www.geeksforgeeks.org/how-to-create-grouped-box-plot-in-plotly/
            // For this to group, we must invoke Chart2D.Chart.BoxPlot once per group
            var charts = new List<GenericChart.GenericChart>();
            foreach (var job in jobs)
            {
                var names = job.SelectMany(p => p.Names).ToArray();
                var data = job.SelectMany(p => p.Data).ToArray();

                charts.Add(Chart2D.Chart.BoxPlot<string, double, string>(X: names, Y: data, Name: job.Key, Jitter: 0.1, BoxPoints: StyleParam.BoxPoints.All));
            }

            Chart.Combine(charts)
                .WithoutVerticalGridlines()
                .WithAxisTitles($"Time ({timeUnit})")
                .WithLayout(title)
                .WithGroupBox()
                .SaveSVG(file, Width: 1000, Height: 600);

            return new[] { file + ".svg" };
        }

        // internal measurements are in nanos
        // https://github.com/dotnet/BenchmarkDotNet/blob/e4d37d03c0b1ef14e7bde224970bd0fc547fd95a/src/BenchmarkDotNet/Templates/BuildPlots.R#L63-L75
        private static string ConvertTime(List<BoxPlotInfo> plots)
        {
            var max = plots.SelectMany(p => p.Data).Max();

            string timeUnit = "ns";
            
            if (max > 1_000_000_000)
            {
                Reduce(plots, 0.000000001);
                timeUnit = "s";
            }
            else if (max > 1_000_000)
            {
                Reduce(plots, 0.000001);
                timeUnit = "ms";
            }
            else if (max > 1_000)
            {
                Reduce(plots, 0.001);
                timeUnit = "μs";
            }

            return timeUnit;
        }

        private static void Reduce(List<BoxPlotInfo> plots, double factor)
        { 
            foreach (var plot in plots)
            {
                for (int i = 0; i < plot.Data.Length; i++)
                {
                    plot.Data[i] *= factor;
                }
            }
        }

        public void ExportToLog(Summary summary, ILogger logger)
        {
            logger.WriteLine("Generated BoxPlot");
        }

        private class BoxPlotInfo
        {
            public BoxPlotInfo(BenchmarkReport r)
            {
                Job = r.BenchmarkCase.Job.ResolvedId;
                var name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo;
                Data = r.AllMeasurements.Where(m => m.IterationMode == BenchmarkDotNet.Engines.IterationMode.Workload && m.IterationStage == BenchmarkDotNet.Engines.IterationStage.Actual).Select(m => m.GetAverageTime().Nanoseconds).ToArray();
                Names = Enumerable.Range(0, Data.Length).Select(_ => name).ToArray();
            }

            public double[] Data { get; set; }

            public string[] Names { get; set; }

            public string Job { get; set; }
        }
    }
}