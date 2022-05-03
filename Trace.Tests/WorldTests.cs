using Xunit;
using Trace.Cameras;
using Trace.Geometry;

namespace Trace.Tests;

public class WorldTests
{
    private Vec VecX = new Vec(1f, 0f, 0f);
    private Vec VecY = new Vec(0f, 1f, 0f);
    private Vec VecZ = new Vec(0f, 0f, 1f);

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
}