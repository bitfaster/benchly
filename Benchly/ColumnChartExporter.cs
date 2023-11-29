using BenchmarkDotNet.Exporters;
using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

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

                var columnChartData = new TraceInfo()
                {
                    Keys = new[] { report.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo },
                    Values = new[] { report.Success ? report.ResultStatistics.Mean : 0 }
                };

                ColumnChartRenderer.Render(new[] { columnChartData }, title, file, Info.Width, Info.Height, false);

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

            var charts = summary.Reports.Select(r => new TraceInfo()
            {
                TraceName = r.BenchmarkCase.Job.ResolvedId,
                Keys = new[] { r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo },
                Values = new[] { r.Success ? r.ResultStatistics.Mean : 0 }
            }).GroupBy(r => r.TraceName);

            var colors = ColorMap.GetColorList(Info);

            // make 1 chart per column so that we can color by bar index. Legend is disabled since it is not needed.
            foreach (var chartData in charts)
            {
                var title = this.Info.Title ?? summary.Title;
                var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-" + chartData.Key + "-columnchart");

                ColorMap.Fill(chartData, colors);
                ColumnChartRenderer.Render(chartData, title, file, Info.Width, Info.Height, false);

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

            var charts = summary.Reports.Select(r => new
            {
                job = r.BenchmarkCase.Job.ResolvedId,
                name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo,
                mean = r.Success ? r.ResultStatistics.Mean : 0
            })
            .GroupBy(r => r.job)
                .Select(job => new TraceInfo() { Values = job.Select(j => j.mean).ToArray(), Keys = job.Select(j => j.name).ToArray(), TraceName = job.Key });

            var colors = ColorMap.GetColorList(Info);
            ColorMap.Fill(charts, colors);
            ColumnChartRenderer.Render(charts, title, file, Info.Width, Info.Height, true);

            return new[] { file + ".svg" };
        }

        private IEnumerable<string> OneParameterCombined(Summary summary)
        {
            var title = this.Info.Title ?? summary.Title;
            var file = Path.Combine(summary.ResultsDirectoryPath, ExporterBase.GetFileName(summary) + "-columnchart");

            var subPlots = summary.Reports
                .Select(r => new
                {
                    param = r.BenchmarkCase.Parameters.PrintInfo,
                    job = r.BenchmarkCase.Job.ResolvedId,
                    name = r.BenchmarkCase.Descriptor.WorkloadMethodDisplayInfo,
                    mean = r.Success ? r.ResultStatistics.Mean : 0
                })
                .GroupBy(r => r.param)
                .Select(bp => new SubPlot()
                {
                    Title = bp.Key,
                    Traces = bp
                        .GroupBy(p => p.job)
                        .Select(j => new TraceInfo() 
                        { 
                            TraceName = j.Key,
                            Values = j.Select(j => j.mean).ToArray(), 
                            Keys = j.Select(j => j.name).ToArray(),
                        }).ToList()
                }).ToList();

            var colors2 = ColorMap.GetColorList(Info);
            ColumnChartRenderer.Render(subPlots, title, file, Info.Width, Info.Height, colors2);

            return new[] { file + ".svg" };
        }

        // internal measurements are in nanos
        // https://github.com/dotnet/BenchmarkDotNet/blob/e4d37d03c0b1ef14e7bde224970bd0fc547fd95a/src/BenchmarkDotNet/Templates/BuildPlots.R#L63-L75
        //private static double ConvertNanosToMs(double nanos)
        //{
        //    return nanos * 0.000001;
        //}
    }
}
