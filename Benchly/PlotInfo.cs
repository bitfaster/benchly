using Plotly.NET;

namespace Benchly
{
    internal class PlotInfo
    {
        public string Title { get; set; }

        public string Colors { get; set; }

        public int Width { get; set; } = 1000;

        public int Height { get; set; } = 600;

        public OutputMode OutputMode { get; set; } = OutputMode.Combined;

        internal Color[] GetColors()
        {
            if (string.IsNullOrEmpty(Colors))
            { 
                return Array.Empty<Color>();
            }

            var split = Colors.Split(',');

            try
            {
                return split.Select(x => ParseColor(x)).ToArray();
            }
            catch 
            {
                return Array.Empty<Color>();
            }
        }

        private Color ParseColor(string x)
        { 
            if (x.StartsWith("#"))
            {
                return Color.fromHex(x);
            }

            var keyword = ColorKeyword.ofKeyWord.Invoke(x);
            var c = Color.fromKeyword(keyword);
            return c;
        }
    }
}
