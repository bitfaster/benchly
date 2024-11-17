using Microsoft.FSharp.Core;
using Plotly.NET.LayoutObjects;
using Plotly.NET;
using static Plotly.NET.StyleParam;

namespace Benchly
{
    internal static class PlotExtensions
    {
        public static GenericChart WithAxisTitles(this GenericChart chart, string yTitle)
        {
            var font = new FSharpOption<Font>(Font.init(Size: new FSharpOption<double>(16)));
            FSharpOption<string> yt = new FSharpOption<string>(yTitle);
            return chart.WithXAxisStyle(Title.init(Font: font)).WithYAxisStyle(Title.init(yt, Font: font));
        }

        public static GenericChart WithAxisTitles(this GenericChart chart, string xTitle, string yTitle)
        {
            var font = new FSharpOption<Font>(Font.init(Size: new FSharpOption<double>(16)));
            FSharpOption<string> xt = new FSharpOption<string>(xTitle);
            FSharpOption<string> yt = new FSharpOption<string>(yTitle);
            return chart.WithXAxisStyle(Title.init(xt, Font: font)).WithYAxisStyle(Title.init(yt, Font: font));
        }

        public static GenericChart WithoutVerticalGridlines(this GenericChart chart)
        {
            var gridColor = new FSharpOption<Color>(Color.fromKeyword(ColorKeyword.Gainsboro));
            var yaxis = LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible>(
                GridColor: gridColor,
                ZeroLineColor: gridColor);

            var axis = LinearAxis.init<IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible, IConvertible>(ShowGrid: new FSharpOption<bool>(false));
            return chart.WithXAxis(axis).WithYAxis(yaxis);
        }

        public static GenericChart WithLayout(this GenericChart chart, string title)
        {
            var font = new FSharpOption<Font>(Font.init(Size: new FSharpOption<double>(24)));
            FSharpOption<Title> t = Title.init(Text: title, X: 0.5, Font: font);
            FSharpOption<Color> plotBGColor = new FSharpOption<Color>(Color.fromKeyword(ColorKeyword.WhiteSmoke));
            Layout layout = Layout.init<IConvertible>(PaperBGColor: plotBGColor, PlotBGColor: plotBGColor, Title: t);
            return chart.WithLayout(layout);
        }

        public static GenericChart WithGroupBox(this GenericChart chart)
        {
            Layout layout = Layout.init<IConvertible>(BoxMode: BoxMode.Group);
            return chart.WithLayout(layout);
        }

        public static GenericChart WithLegendGroup(this GenericChart chart, string groupName, bool showLegend)
        {
            return chart.WithTraceInfo(LegendGroup: new FSharpOption<string>(groupName), ShowLegend: new FSharpOption<bool>(showLegend));
        }
    }
}
