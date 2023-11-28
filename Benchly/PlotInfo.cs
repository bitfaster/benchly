using Plotly.NET;

namespace Benchly
{
    internal class PlotInfo
    {
        public string Title { get; set; }

        public string Colors { get; set; }

        public int Width { get; set; } = 1000;

        public int Height { get; set; } = 600;

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
