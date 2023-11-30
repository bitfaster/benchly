using Microsoft.FSharp.Core;
using Plotly.NET;
using Plotly.NET.ImageExport;
using Plotly.NET.LayoutObjects;
using static Plotly.NET.StyleParam;

namespace Benchly
{
    internal class TraceInfo
    {
        // these are the values
        public double[] Values { get; set; }

        // These are the keys for each value
        public string[] Keys { get; set; }

        // the trace name used for the legend
        public string TraceName { get; set; } 

        public Color MarkerColor { get; set; }
    }

    internal class SubPlot
    {
        public string Title { get; set; }

        public List<TraceInfo> Traces { get; set; }
    }

    internal class ColumnChartRenderer
    {
        public static void Render(IEnumerable<TraceInfo> traces, string title, string file, int width, int height, bool showLegend)
        {
            var timeUnit = TimeNormalization.Normalize(traces);

            var charts = traces
                        .Select((cd, i) => Chart2D.Chart.Column<double, string, string, double, double>(
                            cd.Values,
                            cd.Keys,
                            Name: cd.TraceName,
                            MarkerColor: cd.MarkerColor)
                        .WithLegendGroup(cd.TraceName, showLegend));

            Chart.Combine(charts)
                    .WithAxisTitles($"Latency ({timeUnit})")
                    .WithoutVerticalGridlines()
                    .WithLayout(title)
                    .SaveSVG(file, Width: width, Height: height);
        }

        public static void Render(IEnumerable<SubPlot> subPlot, string title, string file, int width, int height, Color[] colors)
        {
            var timeUnit = TimeNormalization.Normalize(subPlot.SelectMany(sp => sp.Traces));

            bool singleJob = subPlot.SelectMany(sp => sp.Traces).Select(t => t.TraceName).Distinct().Count() == 1;

            // make a grid with 1 row, n columns, where n is number of params
            // y axis only on first chart
            var gridCharts = new List<GenericChart.GenericChart>();

            int paramCount = 0;
            foreach (var plot in subPlot)
            {
                ColorMap.Fill(plot.Traces, colors);
                var charts = new List<GenericChart.GenericChart>();

                // Group the legends, then only show the first for each group
                // https://stackoverflow.com/questions/60751008/sharing-same-legends-for-subplots-in-plotly
                foreach (var trace in plot.Traces)
                {
                    var chart2 = Chart2D.Chart
                        .Column<double, string, string, double, double>(trace.Values, trace.Keys, Name: trace.TraceName, MarkerColor: trace.MarkerColor)
                        .WithLegendGroup(trace.TraceName, paramCount == 0 && !singleJob);
                    charts.Add(chart2);
                }

                gridCharts.Add(Chart.Combine(charts));
                paramCount++;
            }

            // https://github.com/plotly/Plotly.NET/issues/387
            double xWidth = 1.0d / subPlot.Count();
            double xMidpoint = xWidth / 2.0d;
            double[] xs = subPlot.Select((_, index) => xMidpoint + (xWidth * index)).ToArray();

            var annotations = subPlot.Select((p, index) => Annotation.init<double, double, string, string, string, string, string, string, string, string>(
                X: xs[index],
                Y: 1, // -0.1, bottom breaks layout if the x labels are too long
                XAnchor: StyleParam.XAnchorPosition.Center,
                ShowArrow: false,
                YAnchor: StyleParam.YAnchorPosition.Bottom,
                Text: p.Title.ToString(),
                XRef: "paper",
                YRef: "paper"
            ));

            // this couples all the charts on the same row to have the same y axis
            var pattern = new FSharpOption<LayoutGridPattern>(LayoutGridPattern.Coupled);

            Chart
                .Grid<IEnumerable<GenericChart.GenericChart>>(1, subPlot.Count(), Pattern: pattern).Invoke(gridCharts)
                .WithAnnotations(annotations)
                .WithoutVerticalGridlines()
                .WithAxisTitles($"Time ({timeUnit})")
                .WithLayout(title)
                .SaveSVG(file, Width: width, Height: height);
        }
    }
}
