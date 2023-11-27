using BenchmarkDotNet.Reports;
using Plotly.NET;

namespace Benchly
{
    internal class ColorMap
    {
        internal static Dictionary<string, Color> GetJobColors(Summary summary, PlotInfo info)
        {
            var jobs = summary.Reports.Select(r => r.BenchmarkCase.Job.ResolvedId).Distinct().ToList();

            var colorMap = new Dictionary<string, Color>();

            var colors = info.GetColors();

            // give some default colors
            if (colors.Length == 0)
            {
                colors = new[] { Color.fromKeyword(ColorKeyword.IndianRed), Color.fromKeyword(ColorKeyword.Salmon) };
            }

            for (int i = 0; i < jobs.Count; i++)
            {
                colorMap.Add(jobs[i], colors[i % colors.Length]);
            }

            return colorMap;
        }
    }
}
