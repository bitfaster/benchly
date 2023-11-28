using FluentAssertions;
using Plotly.NET;

namespace Benchly.UnitTests
{
    public class PlotInfoTests
    {
        [Fact]
        public void WhenColorIsInvalidStringGetColorsIsEmpty()
        { 
            var info = new PlotInfo();
            info.Colors = "foo";

            var colors = info.GetColors();

            colors.Should().NotBeNull();
            colors.Should().BeEmpty();
        }

        [Fact]
        public void WhenColorIsValidStringGetColorsReturnsColor()
        {
            var info = new PlotInfo();
            info.Colors = "firebrick";

            var colors = info.GetColors();

            colors.Should().NotBeNull();
            colors.Length.Should().Be(1);
            var argb = colors[0].Value.Should().BeOfType<ARGB>().Subject;

            argb.A.Should().Be(255);
            argb.R.Should().Be(178);
            argb.G.Should().Be(34);
            argb.B.Should().Be(34);
        }

        [Fact]
        public void WhenColorIsHexItIsMapped()
        {
            var info = new PlotInfo();
            info.Colors = "#FFFFFF,#000000";

            var colors = info.GetColors();

            colors.Should().NotBeNull();
            colors.Length.Should().Be(2);
            var argb = colors[0].Value.Should().BeOfType<ARGB>().Subject;

            argb.A.Should().Be(255);
            argb.R.Should().Be(255);
            argb.G.Should().Be(255);
            argb.B.Should().Be(255);

            var argb2 = colors[1].Value.Should().BeOfType<ARGB>().Subject;

            argb2.A.Should().Be(255);
            argb2.R.Should().Be(0);
            argb2.G.Should().Be(0);
            argb2.B.Should().Be(0);
        }
    }
}
