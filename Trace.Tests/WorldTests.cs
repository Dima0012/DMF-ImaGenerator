using Trace.Cameras;
using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class WorldTests
{
    private Vec VecX = new(1f, 0f, 0f);
    private Vec VecY = new(0f, 1f, 0f);
    private Vec VecZ = new(0f, 0f, 1f);

    [Fact]
    public void TestRayIntersections()
    {
        var world = new World();
        var sphere1 = new Sphere(Transformation.translation(VecX * 2));
        var sphere2 = new Sphere(Transformation.translation(VecX * 8));
        world.add(sphere1);
        world.add(sphere2);
        var ray1 = new Ray(new Point(0, 0, 0), VecX);
        var intersection1 = world.ray_intersection(ray1);
        Assert.True(intersection1 != null);
        Assert.True(intersection1?.WorldPoint.is_close(new Point(1, 0, 0)));

        var ray2 = new Ray(new Point(10f, 0f, 0f), VecX.neg());
        var intersection2 = world.ray_intersection(ray2);
        Assert.True(intersection2 != null);
        Assert.True(intersection2?.WorldPoint.is_close(new Point(9, 0, 0)));
    }

    [Fact]
    public void TestQuickRayIntersection()
    {
        var world = new World();

        var sphere1 = new Sphere(Transformation.translation(VecX * 2));
        var sphere2 = new Sphere(Transformation.translation(VecX * 8));
        world.add(sphere1);
        world.add(sphere2);

        Assert.False(world.is_point_visible(new Point(10.0f, 0.0f, 0.0f),
            new Point(0.0f, 0.0f, 0.0f)));
        Assert.False(world.is_point_visible(new Point(5.0f, 0.0f, 0.0f),
            new Point(0.0f, 0.0f, 0.0f)));
        Assert.True(world.is_point_visible(new Point(5.0f, 0.0f, 0.0f),
            new Point(4.0f, 0.0f, 0.0f)));
        Assert.True(world.is_point_visible(new Point(0.5f, 0.0f, 0.0f),
            new Point(0.0f, 0.0f, 0.0f)));
        Assert.True(world.is_point_visible(new Point(0.0f, 10.0f, 0.0f),
            new Point(0.0f, 0.0f, 0.0f)));
        Assert.True(world.is_point_visible(new Point(0.0f, 0.0f, 10.0f),
            new Point(0.0f, 0.0f, 0.0f)));
    }
}