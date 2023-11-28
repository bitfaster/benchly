using FluentAssertions;
using Plotly.NET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Benchly.UnitTests
{
    public class PlotInfoTests
    {
        [Fact]
        public void WhenColorIsStringItIsMapped()
        { 
            var info = new PlotInfo();
            info.Colors = "foo";

            var colors = info.GetColors();

            colors.Should().NotBeNull();
            colors.Length.Should().Be(1);
            colors[0].Value.Should().Be("foo");
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
