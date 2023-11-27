using Plotly.NET;

namespace Benchly
{
    internal class PlotInfo
    {
        public string Title { get; set; }

        public string Colors { get; set; }

        internal Color[] GetColors()
        {
            if (string.IsNullOrEmpty(Colors))
            { 
                return Array.Empty<Color>();
            }

            var split = Colors.Split(',');

            try
            {
                return split.Select(x => Color.fromString(x)).ToArray();
            }
            catch 
            {
                return Array.Empty<Color>();
            }
        }
    }
}
