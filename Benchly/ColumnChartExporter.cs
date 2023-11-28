using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

using Plotly.NET;
using Plotly.NET.ImageExport;
using Plotly.NET.LayoutObjects;
using static Plotly.NET.StyleParam;

using Microsoft.FSharp.Core;

namespace Benchly
{
    internal class ColumnChartExporter : IExporter
    {
        public PlotInfo Info { get; set; } = new PlotInfo();

        public string Name => nameof(ColumnChartExporter);

        public IEnumerable<string> ExportToFiles(Summary summary, ILogger consoleLogger)
        {
            if (summary.Reports.Length == 0)
            { 
                return Array.Empty<string>();
            }

            return Info.OutputMode switch
            {
                OutputMode.PerMethod => PerMethod(summary),
                OutputMode.PerJob => PerJob(summary),
                OutputMode.Combined => Combined(summary),
                _ => Array.Empty<string>(),
            };
        }

        public void ExportToLog(Summary summary, ILogger logger)
        {
        }

        // Revisit this in terms of params:
        // This would make most sense if used with params, so that you
        // could look at the results for all param values for each method
        private IEnumerable<string> PerMethod(Summary summary)
        {
            var files = new List<string>();

            foreach (var report in summary.Reports) 
            {
                if (!report.Success)
                {
                    continue;
                }

                int paramCount = report.BenchmarkCase.Parameters.Count;

                var title = this.Info.Title ?? summary.Title;
                var fileSlug = paramCount == 0
                    ? report.BenchmarkCase.Job.ResolvedId + "-" + report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo
                    : report.BenchmarkCase.Job.ResolvedId + "-" + report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo + "-" + report.BenchmarkCase.Parameters.PrintInfo;

                fileSlug += "-columnchart";
                var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + fileSlug);

                var mean = new[] { ConvertNanosToMs(report.ResultStatistics.Mean) };
                var name = report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo;

                Chart2D.Chart.Column<double, string, string, double, double>(mean, Name: name)
                    .WithAxisTitles("Time (ms)")
                    .WithoutVerticalGridlines()
                    .WithLayout(title)
                    .SaveSVG(file, Width: Info.Width, Height: Info.Height);

                files.Add(file + ".svg");
            }
            return files;
        }
        private IEnumerable<string> PerJob(Summary summary)
        {
            // don't support params for now
            if (summary.Reports[0].BenchmarkCase.HasParameters)
            {
                return Array.Empty<string>();
            }

            var files = new List<string>();

            var jobs = summary.Reports.Select(r => new
            {
                job = r.BenchmarkCase.Job.ResolvedId,
                name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo,
                mean = r.Success ? ConvertNanosToMs(r.ResultStatistics.Mean) : 0
            }).GroupBy(r => r.job);

            foreach (var job in jobs)
            {
                var title = this.Info.Title ?? summary.Title;
                var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-" + job.Key + "-columnchart");

                var colors = ColorMap.GetColorList(Info);

                // make 1 chart per column so that we can color by bar index. Legend is disabled since it is not needed.
                var charts = job
                    .Select((j, i) => Chart2D.Chart.Column<double, string, string, double, double>(
                        new[] { j.mean }, 
                        new[] { j.name }, 
                        Name: job.Key, 
                        MarkerColor: colors[i % colors.Length])
                    .WithLegendGroup(job.Key, false));

                Chart.Combine(charts)
                    .WithAxisTitles("Time (ms)")
                    .WithoutVerticalGridlines()
                    .WithLayout(title)
                    .SaveSVG(file, Width: Info.Width, Height: Info.Height);

                files.Add(file + ".svg");
            }

            return files;
        }

        private IEnumerable<string> Combined(Summary summary)
        {
            if (summary.Reports[0].BenchmarkCase.HasParameters)
            {
                int paramCount = summary.Reports[0].BenchmarkCase.Parameters.Count;

                if (paramCount == 1)
                {
                    return OneParameterCombined(summary);
                }

                // we only support 0 or 1 params
                return Array.Empty<string>();
            }

            return NoParameterCombined(summary);
        }

        private IEnumerable<string> NoParameterCombined(Summary summary)
        {
            var title = this.Info.Title ?? summary.Title;
            var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-columnchart");

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

            chart.SaveSVG(file, Width: Info.Width, Height: Info.Height);

            return new[] { file + ".svg" };
        }

        private IEnumerable<string> OneParameterCombined(Summary summary)
        {
            var title = this.Info.Title ?? summary.Title;
            var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-columnchart");

            // make a grid with 1 row, n columns, where n is number of params
            // y axis only on first chart
            var gridCharts = new List<GenericChart.GenericChart>();

            var colors = ColorMap.GetJobColors(summary, this.Info);

            var byParam = summary.Reports.Select(r => new
            {
                param = r.BenchmarkCase.Parameters.PrintInfo,
                job = r.BenchmarkCase.Job.ResolvedId,
                name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo,
                mean = r.Success ? ConvertNanosToMs(r.ResultStatistics.Mean) : 0
            }).GroupBy(r => r.param);

            int paramCount = 0;
            foreach (var param in byParam)
            {
                var charts = new List<GenericChart.GenericChart>();
                var jobs = param.GroupBy(p => p.job);

                // Group the legends, then only show the first for each group
                // https://stackoverflow.com/questions/60751008/sharing-same-legends-for-subplots-in-plotly
                foreach (var job in jobs)
                {
                    var chart2 = Chart2D.Chart
                        .Column<double, string, string, double, double>(job.Select(j => j.mean), job.Select(j => j.name).ToArray(), Name: job.Key, MarkerColor: colors[job.Key])
                        .WithLegendGroup(job.Key, paramCount == 0);
                    charts.Add(chart2);
                }

                gridCharts.Add(Chart.Combine(charts));
                paramCount++;
            }

            // https://github.com/plotly/Plotly.NET/issues/387
            double xWidth = 1.0d / byParam.Count();
            double xMidpoint = xWidth / 2.0d;
            double[] xs = byParam.Select((_, index) => xMidpoint + (xWidth * index)).ToArray();

            var annotations = byParam.Select((p, index) => Annotation.init<double, double, string, string, string, string, string, string, string, string>(
                X: xs[index],
                Y: -0.1,
                XAnchor: StyleParam.XAnchorPosition.Center,
                ShowArrow: false,
                YAnchor: StyleParam.YAnchorPosition.Bottom,
                Text: p.Key.ToString(),
                XRef: "paper",
                YRef: "paper"
            ));

            // this couples all the charts on the same row to have the same y axis
            var pattern = new FSharpOption<LayoutGridPattern>(LayoutGridPattern.Coupled);

            Chart
                .Grid<IEnumerable<GenericChart.GenericChart>>(1, byParam.Count(), Pattern: pattern).Invoke(gridCharts)
                .WithAnnotations(annotations)
                .WithoutVerticalGridlines()
                .WithAxisTitles("Time (ms)")
                .WithLayout(title)
                .SaveSVG(file, Width: Info.Width, Height: Info.Height);

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
