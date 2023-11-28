using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Exporters;

namespace Benchly
{
    /// <summary>
    /// The chart output mode.
    /// </summary>
    public enum OutputMode
    {
        /// <summary>
        /// Output a chart per benchmark method.
        /// </summary>
        PerMethod,

        /// <summary>
        /// Output a chart per benchmark job. This groups methods by job.
        /// </summary>
        PerJob,

        /// <summary>
        /// Output a combined chart, showing all jobs and methods.
        /// </summary>
        Combined
    }

    /// <summary>
    /// Export a box plot.
    /// </summary>
    public sealed class BoxPlotAttribute : PlotBaseAttribute
    {
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
    /// Export a column chart.
    /// </summary>
    public sealed class ColumnChartAttribute : PlotBaseAttribute
    {
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
        /// Gets or sets the output mode.
        /// </summary>
        public OutputMode Output
        {
            get => plotInfo.OutputMode;
            set => plotInfo.OutputMode = value;
        }

        /// <summary>
        ///  Initializes a new instance of the <see cref="ColumnChartAttribute"/> class.
        /// </summary>
        public ColumnChartAttribute()
            : base(new ColumnChartExporter())
        {
            var exp = Config.GetExporters().OfType<ColumnChartExporter>().Single();
            exp.Info = plotInfo;
        }
    }

    /// <summary>
    /// Export a histogram plot.
    /// </summary>
    public sealed class HistogramAttribute : PlotBaseAttribute
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="HistogramAttribute"/> class.
        /// </summary>
        public HistogramAttribute()
            : base(new HistogramExporter())
        {
            var exp = Config.GetExporters().OfType<HistogramExporter>().Single();
            exp.Info = plotInfo;
        }
    }

    /// <summary>
    /// Export a histogram plot.
    /// </summary>
    public sealed class TimelineAttribute : PlotBaseAttribute
    {
        /// <summary>
        ///  Initializes a new instance of the <see cref="TimelineAttribute"/> class.
        /// </summary>
        public TimelineAttribute()
            : base(new TimelineExporter())
        {
            var exp = Config.GetExporters().OfType<TimelineExporter>().Single();
            exp.Info = plotInfo;
        }
    }

    /// <summary>
    /// Base class for plot attributes.
    /// </summary>
    public abstract class PlotBaseAttribute : ExporterConfigBaseAttribute
    {
        internal readonly PlotInfo plotInfo = new PlotInfo();

        /// <summary>
        /// Gets or sets the width of the plot.
        /// </summary>
        public int Width
        {
            get => plotInfo.Width;
            set => plotInfo.Width = value;
        }

        /// <summary>
        /// Gets or sets the height of the plot.
        /// </summary>
        public int Height
        {
            get => plotInfo.Height;
            set => plotInfo.Height = value;
        }

        internal PlotBaseAttribute(IExporter exporter)
            : base(exporter)
        {
        }
    }
}
