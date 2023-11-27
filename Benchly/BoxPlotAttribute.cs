using BenchmarkDotNet.Attributes;

namespace Benchly
{
    /// <summary>
    /// Export a box plot.
    /// </summary>
    public sealed class BoxPlotAttribute : ExporterConfigBaseAttribute
    {
        private readonly PlotInfo plotInfo = new PlotInfo();

        /// <summary>
        /// Gets or sets the title of the plot.
        /// </summary>
        public string Title 
        { 
            get => plotInfo.Title;
            set => plotInfo.Title = value;
        }

        /// <summary>
        /// Colors defined by a comma separated list of web color keywords, e.g. White -> "white" (see //https://www.w3.org/TR/2011/REC-SVG11-20110816/types.html#ColorKeywords)
        /// </summary>
        public string Colors
        {
            get => plotInfo.Colors;
            set => plotInfo.Colors = value;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="BoxPlotAttribute"/> class.
        /// </summary>
        public BoxPlotAttribute() 
            : base(new BoxPlotExporter())
        {
            var exp = Config.GetExporters().OfType<BoxPlotExporter>().Single();
            exp.Info = plotInfo;
        }
    }

    /// <summary>
    /// Export a bar plot.
    /// </summary>
    public sealed class BarPlotAttribute : ExporterConfigBaseAttribute
    {
        private readonly PlotInfo plotInfo = new PlotInfo();

        /// <summary>
        /// Gets or sets the title of the plot.
        /// </summary>
        public string Title
        {
            get => plotInfo.Title;
            set => plotInfo.Title = value;
        }

        /// <summary>
        /// Colors defined by a comma separated list of web color keywords, e.g. White -> "white" (see //https://www.w3.org/TR/2011/REC-SVG11-20110816/types.html#ColorKeywords)
        /// </summary>
        public string Colors
        {
            get => plotInfo.Colors;
            set => plotInfo.Colors = value;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="BarPlotAttribute"/> class.
        /// </summary>
        public BarPlotAttribute()
            : base(new BarPlotExporter())
        {
            var exp = Config.GetExporters().OfType<BarPlotExporter>().Single();
            exp.Info = plotInfo;
        }
    }

    /// <summary>
    /// Export a histogram plot.
    /// </summary>
    public sealed class HistogramAttribute : ExporterConfigBaseAttribute
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="BarPlotAttribute"/> class.
        /// </summary>
        public HistogramAttribute()
            : base(new HistogramExporter())
        {
        }
    }
}
