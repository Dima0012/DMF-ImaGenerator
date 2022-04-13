using System;
using Xunit;
using Trace;
using Trace.Cameras;
using Trace.Geometry;

namespace Trace.Tests;

public class RayTests
{
    [Fact]
    public void TestIsClose()
    {
        var ray1 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        var ray2 = new Ray(new Point(1.0f, 2.0f, 3.0f), new Vec(5.0f, 4.0f, -1.0f));
        var ray3 = new Ray(new Point(5.0f, 1.0f, 4.0f), new Vec(3.0f, 9.0f, 4.0f));
        
        Assert.True(ray1.is_close(ray2));
        Assert.False(ray1.is_close(ray3));
    }

    [Fact]
    public void TestAt()
    {
        var ray = new Ray(new Point(1.0f, 2.0f, 4.0f), new Vec(4.0f, 2.0f, 1.0f));
        Assert.True(ray.at(0.0f).is_close(ray.Origin));
        Assert.True(ray.at(1.0f).is_close(new Point(5.0f, 4.0f, 5.0f)));
        Assert.True(ray.at(2.0f).is_close(new Point(9.0f, 6.0f, 6.0f)));
    }
}