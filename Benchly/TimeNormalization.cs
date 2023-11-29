
namespace Benchly
{
    internal class TimeNormalization
    {
        public static string Normalize(IEnumerable<TraceInfo> traces)
        {
            var max = traces.SelectMany(t => t.Values).Max();

            string timeUnit = "ns";

            if (max > 1_000_000_000)
            {
                Reduce(traces, 0.000000001);
                timeUnit = "s";
            }
            else if (max > 1_000_000)
            {
                Reduce(traces, 0.000001);
                timeUnit = "ms";
            }
            else if (max > 1_000)
            {
                Reduce(traces, 0.001);
                timeUnit = "μs";
            }

            return timeUnit;
        }

        private static void Reduce(IEnumerable<TraceInfo> traces, double factor)
        {
            foreach (var plot in traces)
            {
                for (int i = 0; i < plot.Values.Length; i++)
                {
                    plot.Values[i] *= factor;
                }
            }
        }
    }
}
