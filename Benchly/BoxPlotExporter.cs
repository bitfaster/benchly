using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Jobs;
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
            var file = Path.Combine(summary.ResultsDirectoryPath, summary.Title + "boxplot");

            var plots = summary.Reports.Select(r => new BoxPlotInfo(r)).ToList();
            string timeUnit = ConvertTime(plots);

            var boxplots = plots.Select(p => Chart2D.Chart.BoxPlot<string, double, string>(X: p.Names, Y: p.Data, Name: p.Job, Jitter: 0.1, BoxPoints: StyleParam.BoxPoints.All));

            //foreach (var r in summary.Reports)
            //{
            //   // var job = r.BenchmarkCase.Job.DisplayInfo;
            //   // var name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo;
            //   //// var data = r.AllMeasurements.Where(m => m.IterationMode == BenchmarkDotNet.Engines.IterationMode.Workload).Select(m => m.Nanoseconds).ToArray();

            //   // var (data, timeUnit) = ConvertTime(r);

            //   // var names = Enumerable.Range(0, data.Length).Select(_ => name).ToArray();

            //    var cc = Chart.BoxPlot<string, double, string>(X: names, Y: data, Name: job, Jitter: 0.1, BoxPoints: StyleParam.BoxPoints.All);
            //    boxplots.Add(cc);
            //}

            Chart.Combine(boxplots)
                .WithoutVerticalGridlines()
                .WithAxisTitles($"Time ({timeUnit})")
                .WithLayout(title)
                .SaveSVG(file, Width: 1000, Height: 600);

            return new[] { file };
        }

        // internal measurements are in nanos
        // https://github.com/dotnet/BenchmarkDotNet/blob/e4d37d03c0b1ef14e7bde224970bd0fc547fd95a/src/BenchmarkDotNet/Templates/BuildPlots.R#L63-L75
        private static string ConvertTime(List<BoxPlotInfo> plots)
        {
            var min = plots.SelectMany(p => p.Data).Max();

            string timeUnit = "ns";
            
            if (min > 1_000_000_000)
            {
                Reduce(plots, 0.000000001);
                timeUnit = "s";
            }
            else if (min > 1_000_000)
            {
                Reduce(plots, 0.000001);
                timeUnit = "ms";
            }
            else if (min > 1_000)
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
                Job = r.BenchmarkCase.Job.DisplayInfo;
                var name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo;
                Data = r.AllMeasurements.Where(m => m.IterationMode == BenchmarkDotNet.Engines.IterationMode.Workload).Select(m => m.Nanoseconds).ToArray();
                Names = Enumerable.Range(0, Data.Length).Select(_ => name).ToArray();
            }

            public double[] Data { get; set; }

            public string[] Names { get; set; }

            public string Job { get; set; }
        }
    }
}