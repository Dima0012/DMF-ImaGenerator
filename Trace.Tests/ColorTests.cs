using System;
using Xunit;
using Trace;

namespace Trace.Tests;

public class ColorTests
{
    [Fact]

    public void TestClose()
    {
        Color a = new Color(1.0f, 2.0f, 3.0f);
        Assert.True(a.is_close(new Color(1.0f, 2.0f, 3.0f)));
        Assert.False(a.is_close(new Color(3.0f, 4.0f, 5.0f)));

    }
    [Fact]
    public void TestAdd()
    {
        Color a1 = new Color(1.0f, 2.0f, 3.0f);
        Color b1 = new Color(5.0f, 7.0f, 9.0f);
        Assert.True((a1 + b1).is_close(new Color(6.0f, 9.0f, 12.0f)));
    }
    [Fact]
    public void TestDiff()
    {
        Color a1 = new Color(1.0f, 2.0f, 3.0f);
        Color b1 = new Color(5.0f, 7.0f, 9.0f);
        Assert.True((a1 - b1).is_close(new Color(-4.0f, -5.0f, -6.0f)));
    }
    [Fact]
    public void TestProd()
    {
        Color a1 = new Color(1.0f, 2.0f, 3.0f);
        Color b1 = new Color(5.0f, 7.0f, 9.0f);
        Assert.True((a1 * b1).is_close(new Color(5.0f, 14.0f, 27.0f)));
    }
    [Fact]
    public void TestScalarProd()
    {
        Color a1 = new Color(1.0f, 2.0f, 3.0f);
        Assert.True((a1*2.0f).is_close(new Color(2.0f, 4.0f, 6.0f)));
        Assert.True((2.0f*a1).is_close(new Color(2.0f, 4.0f, 6.0f)));
    }
}