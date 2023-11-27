using BenchmarkDotNet.Attributes;
using BenchmarkDotNet.Configs;

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
        /// Initializes a new instance of the <see cref="BoxPlotAttribute"/> class.
        /// </summary>
        public BoxPlotAttribute() 
            : base(new BoxPlotExporter(new PlotInfo()))
        {
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
        ///  Initializes a new instance of the <see cref="BarPlotAttribute"/> class.
        /// </summary>
        public BarPlotAttribute()
            : base(new BarPlotExporter(new PlotInfo()))
        {
        }
    }
}
