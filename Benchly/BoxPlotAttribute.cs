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
            : base()
        {
            this.Config.AddExporter(new BoxPlotExporter(this.plotInfo));
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
            : base()
        {
            this.Config.AddExporter(new BarPlotExporter(this.plotInfo));
        }
    }
}
