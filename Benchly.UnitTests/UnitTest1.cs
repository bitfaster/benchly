using BenchmarkDotNet.Loggers;

namespace Benchly.UnitTests
{
    public class UnitTest1
    {
        [Fact]
        public void Test1()
        {
            var bench = new TestBenchmarkRunner();
            var summary = bench.GetSummary();
            var boxPlotExporter = new BoxPlotExporter();
            boxPlotExporter.Info.Title = "Box Plot";
            var barPlotExporter = new BarPlotExporter();
            barPlotExporter.Info.Title = "Bar Plot";
            var files1 = boxPlotExporter.ExportToFiles(summary, NullLogger.Instance);
            var files2 = barPlotExporter.ExportToFiles(summary, NullLogger.Instance);
        }
    }
}