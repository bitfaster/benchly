using BenchmarkDotNet.Helpers;
using BenchmarkDotNet.Reports;

namespace Benchly
{
    internal class ExporterBase
    {
        public static string GetFileName(Summary summary)
        {
            // we can't use simple name here, because user might be running benchmarks for a library,  which defines few types with the same name
            // and reports the results per type, so every summary is going to contain just single benchmark
            // and we can't tell here if there is a name conflict or not
            var targets = summary.BenchmarksCases.Select(b => b.Descriptor.Type).Distinct().ToArray();

            if (targets.Length == 1)
                return FolderNameHelper.ToFolderName(targets.Single());

            return summary.Title;
        }
    }
}
