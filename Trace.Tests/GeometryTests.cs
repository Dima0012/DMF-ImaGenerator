using System;
using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class GeometryTests
{
    [Fact]
    public void TestVec()
    {
        var a = new Vec(1.0f, 2.0f, 3.0f);
        var b = new Vec(4.0f, 6.0f, 8.0f);

        Assert.True(a.is_close(a));
        Assert.False(a.is_close(b));
    }

    [Fact]
    public void TestVecOperations()
    {
        var a = new Vec(1.0f, 2.0f, 3.0f);
        var b = new Vec(4.0f, 6.0f, 8.0f);

        Assert.True(a.neg().is_close(new Vec(-1.0f, -2.0f, -3.0f)));
        Assert.True((a + b).is_close(new Vec(5.0f, 8.0f, 11.0f)));
        Assert.True((b - a).is_close(new Vec(3.0f, 4.0f, 5.0f)));
        Assert.True((a * 2).is_close(new Vec(2.0f, 4.0f, 6.0f)));
        Assert.True(Math.Abs(40.0 - a * b) < 10e-5);
        Assert.True(a.vec_prod(b).is_close(new Vec(-2.0f, 4.0f, -2.0f)));
        Assert.True(b.vec_prod(a).is_close(new Vec(2.0f, -4.0f, 2.0f)));
        Assert.True(Math.Abs(14.0 - a.squared_norm()) < 10e-5);
        Assert.True(Math.Abs(14.0 - a.norm() * a.norm()) < 10e-5);
    }

    [Fact]
    public void TestPoint()
    {
        var a = new Point(1.0f, 2.0f, 3.0f);
        var b = new Point(4.0f, 6.0f, 8.0f);
        Assert.True(a.is_close(a));
        Assert.False(a.is_close(b));
    }

    [Fact]
    public void TestPointOperations()
    {
        var p1 = new Point(1.0f, 2.0f, 3.0f);
        var v = new Vec(4.0f, 6.0f, 8.0f);
        var p2 = new Point(4.0f, 6.0f, 8.0f);

        Assert.True((p1 * 2).is_close(new Point(2.0f, 4.0f, 6.0f)));
        Assert.True((p1 + v).is_close(new Point(5.0f, 8.0f, 11.0f)));
        Assert.True((p2 - p1).is_close(new Vec(3.0f, 4.0f, 5.0f)));
        Assert.True((p1 - v).is_close(new Point(-3.0f, -4.0f, -5.0f)));
    }
}