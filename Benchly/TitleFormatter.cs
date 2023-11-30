using BenchmarkDotNet.Reports;

namespace Benchly
{
    internal class TitleFormatter
    {
        public static string Format(PlotInfo Info, Summary summary, string currentJob)
        {
            if (!string.IsNullOrEmpty(Info.Title))
            {
                var title = Info.Title;

                return title.Replace("{JOB}", currentJob);
            }
            
            return summary.Title;
        }
    }
}
