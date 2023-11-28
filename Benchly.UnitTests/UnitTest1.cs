using BenchmarkDotNet.Loggers;
using BenchmarkDotNet.Reports;

namespace Benchly.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void TestNoParams()
        {
            var bench = new TestBenchmarkRunner();
            var summary = bench.GetSummary<Md5VsSha256>();
            RunExporters(summary);
        }

        [Fact]
        public void TestParams()
        {
            var bench = new TestBenchmarkRunner();
            var summary = bench.GetSummary<Md5VsSha256Params>();
            RunExporters(summary);
        }

        private static void RunExporters(Summary summary)
        {
            var boxPlotExporter = new BoxPlotExporter();
            boxPlotExporter.Info.Title = "Box Plot";
            var columnChartExporter = new ColumnChartExporter();
            columnChartExporter.Info.Title = "Column Chart";
            var histExporter = new HistogramExporter();
            var timelineExporter = new TimelineExporter();
            var files1 = boxPlotExporter.ExportToFiles(summary, NullLogger.Instance);
            var files2 = columnChartExporter.ExportToFiles(summary, NullLogger.Instance);
            var files3 = histExporter.ExportToFiles(summary, NullLogger.Instance);
            var files4 = timelineExporter.ExportToFiles(summary, NullLogger.Instance);
        }
    }
}