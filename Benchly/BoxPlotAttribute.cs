using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

namespace Benchly
{
    public sealed class BoxPlotAttribute : ExporterConfigBaseAttribute
    {
        private readonly PlotInfo plotInfo = new PlotInfo();

        public string? Title 
        { 
            get => plotInfo.Title;
            set => plotInfo.Title = value;
        }

        // colorscheme as list of colors

        // width
        // height

        // include error bars

        // include distribution

        // group by column: job, size? Or have different attrib for grouping, then can apply multiple attribs
        // E.g.
        // [BarPlot]
        // [JobBarPlot]
        // [BoxPlot]
        // [JobBoxPlot]
        // [BarPlot(GroupBy="Size")]
        // [DistributionPlot]

        public BoxPlotAttribute() 
            : base()
        {
            this.Config.AddExporter(new BoxPlotExporter(this.plotInfo));
        }
    }

    public sealed class BarPlotAttribute : ExporterConfigBaseAttribute
    {
        private readonly PlotInfo plotInfo = new PlotInfo();

        public string? Title
        {
            get => plotInfo.Title;
            set => plotInfo.Title = value;
        }

        public BarPlotAttribute()
            : base()
        {
            this.Config.AddExporter(new BarPlotExporter(this.plotInfo));
        }
    }

    [BoxPlot(Title = "asdas")]
    public class BenchmarkFake
    { 
    
    }
}
