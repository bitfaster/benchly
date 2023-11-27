using BenchmarkDotNet.Configs;
using BenchmarkDotNet.Exporters;
using System;
using System.Collections.Generic;
using System.Text;

namespace Benchly
{
    /// <summary>
    /// Extension methods for configuring exporters.
    /// </summary>
    public static class ConfigExt
    {
        /// <summary>
        /// Add the benchly exporters.
        /// </summary>
        public static IConfig AddPlotExporters(this IConfig config)
        {
            config.AddExporter(new BoxPlotExporter(new PlotInfo()));
            config.AddExporter(new BarPlotExporter(new PlotInfo()));

            return config;
        }
    }
}
