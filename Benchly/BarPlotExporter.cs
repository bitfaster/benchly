using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Reports;
using Plotly.NET.ImageExport;
using BenchmarkDotNet.Loggers;
using Plotly.NET;
using Plotly.NET.LayoutObjects;

namespace Benchly
{
    internal class BarPlotExporter : IExporter
    {
        public PlotInfo Info { get; set; } = new PlotInfo();

        public string Name => nameof(BarPlotExporter);

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            if (summary.Reports.Length == 0)
            { 
                return Array.Empty<string>();
            }

            if (summary.Reports[0].BenchmarkCase.HasParameters)
            {
                int paramCount = summary.Reports[0].BenchmarkCase.Parameters.Count;

                if (paramCount == 1)
                {
                    return OneParameter(summary);
                }
            }

            return NoParameter(summary);
        }

        public void ExportToLog(Summary summary, ILogger logger)
        {
        }

        private IEnumerable<string> NoParameter(Summary summary)
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

        private IEnumerable<string> OneParameter(Summary summary)
        {
            var title = this.Info.Title ?? summary.Title;
            var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-barplot");

            // make a grid with 1 row, n columns, where n is number of params
            // y axis only on first chart
            var gridCharts = new List<GenericChart.GenericChart>();

            var byParam = summary.Reports.Select(r => new
            {
                param = r.BenchmarkCase.Parameters[0].Value,
                job = r.BenchmarkCase.Job.ResolvedId,
                name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo,
                mean = r.Success ? ConvertNanosToMs(r.ResultStatistics.Mean) : 0
            }).GroupBy(r => r.param);

            foreach (var param in byParam)
            {
                var charts = new List<GenericChart.GenericChart>();
                var jobs = param.GroupBy(p => p.job);

                // For this to group, we must invoke Chart2D.Chart.Column once per group
                foreach (var job in jobs)
                {
                    var chart2 = Chart2D.Chart.Column<double, string, string, double, double>(job.Select(j => j.mean), job.Select(j => j.name).ToArray(), Name: job.Key);
                    charts.Add(chart2);
                }

                gridCharts.Add(Chart.Combine(charts));
            }

            // https://github.com/plotly/Plotly.NET/issues/387
            double xWidth = 1.0d / byParam.Count();
            double xMidpoint = xWidth / 2.0d;
            double[] xs = byParam.Select((_, index) => xMidpoint + (xWidth * index)).ToArray();

            var annotations = byParam.Select((p, index) => Annotation.init<double, double, string, string, string, string, string, string, string, string>(
                X: xs[index],
                Y: 1,
                XAnchor: StyleParam.XAnchorPosition.Center,
                ShowArrow: false,
                YAnchor: StyleParam.YAnchorPosition.Bottom,
                Text: p.Key.ToString(),
                XRef: "paper",
                YRef: "paper"
            ));

            Chart
                .Grid<IEnumerable<GenericChart.GenericChart>>(1, byParam.Count()).Invoke(gridCharts)
                .WithAnnotations(annotations)
                .WithoutVerticalGridlines()
                .WithLayout(title)
                .SaveSVG(file, Width: 1000, Height: 600);

            return new[] { file + ".svg" };
        }

        // internal measurements are in nanos
        // https://github.com/dotnet/BenchmarkDotNet/blob/e4d37d03c0b1ef14e7bde224970bd0fc547fd95a/src/BenchmarkDotNet/Templates/BuildPlots.R#L63-L75
        private static double ConvertNanosToMs(double nanos)
        {
            return nanos * 0.000001;
        }
    }
}
