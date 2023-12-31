﻿using BenchmarkDotNet.Reports;
using Plotly.NET;

namespace Benchly
{
    internal class ColorMap
    {
        private static Color[] defaults = new[] { Color.fromKeyword(ColorKeyword.IndianRed), Color.fromKeyword(ColorKeyword.Salmon) };

        internal static void Fill(IEnumerable<TraceInfo> chartData, Color[] colors)
        {
            int i = 0;
            foreach (TraceInfo data in chartData) 
            {
                data.MarkerColor = colors[i++ % colors.Length];
            }
        }

        internal static Color[] GetColorList(PlotInfo info)
        {
            var colors = info.GetColors();

            if (colors.Length == 0)
            {
                colors = defaults;
            }

            return colors;
        }

        internal static Dictionary<string, Color> GetJobColors(Summary summary, PlotInfo info)
        {
            var jobs = summary.Reports.Select(r => r.BenchmarkCase.Job.ResolvedId).Distinct().ToList();

            var colorMap = new Dictionary<string, Color>();

            var colors = info.GetColors();

            // give some default colors
            if (colors.Length == 0)
            {
                colors = defaults;
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                colorMap.Add(jobs[i], colors[i % colors.Length]);
            }

            return colorMap;
        }
    }
}
