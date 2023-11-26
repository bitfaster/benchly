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
            var boxPlotExporter = new BoxPlotExporter(new Benchly.PlotInfo() { Title = "Box Plot" });
            var barPlotExporter = new BarPlotExporter(new Benchly.PlotInfo() { Title = "Bar Plot" });
            var files1 = boxPlotExporter.ExportToFiles(summary, NullLogger.Instance);
            var files2 = barPlotExporter.ExportToFiles(summary, NullLogger.Instance);
        }
    }
}