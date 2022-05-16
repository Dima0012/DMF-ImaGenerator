using Xunit;
using Trace.Cameras;
using Trace.Geometry;

namespace Trace.Tests;

public class PlaneTests
{
    private Vec VecX = new Vec(1f, 0f, 0f);
    private Vec VecY = new Vec(0f, 1f, 0f);
    private Vec VecZ = new Vec(0f, 0f, 1f);

    [Fact]
    public void TestHit()
    {
        var plane = new Plane();
        var ray1 = new Ray(new Point(0f, 0f, 1f), VecZ.neg());
        var intersection1 = plane.ray_intersection(ray1);
        Assert.True(intersection1 != null);
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 0.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            new Vec2d(0.0f, 0.0f),
            1.0f,
            ray1,
            plane).is_close(intersection1));

        var ray2 = new Ray(new Point(0f, 0f, 1f), VecZ);
        var intersection2 = plane.ray_intersection(ray2);
        Assert.True(intersection2 == null);
        
        var ray3 = new Ray(new Point(0f, 0f, 1f), VecX);
        var intersection3 = plane.ray_intersection(ray3);
        Assert.True(intersection3 == null);
        
        var ray4 = new Ray(new Point(0f, 0f, 1f), VecY);
        var intersection4 = plane.ray_intersection(ray4);
        Assert.True(intersection4 == null);

    }

    [Fact]
    public void TestTransformation()
    {
        var plane = new Plane(Transformation.rotation_y(90));

        var ray1 = new Ray(new Point(1f, 0f, 0f), VecX.neg());
        var intersection1 = plane.ray_intersection(ray1);
        Assert.True(intersection1 != null);
        Assert.True(new HitRecord(
            new Point(0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            new Vec2d(0.0f, 0.0f),
            1.0f,
            ray1,
            plane).is_close(intersection1));

        var ray2 = new Ray(new Point(0f, 0f, 1f), VecZ);
        var intersection2 = plane.ray_intersection(ray2);
        Assert.True(intersection2 == null);

        var ray3 = new Ray(new Point(0f, 0f, 1f), VecX);
        var intersection3 = plane.ray_intersection(ray3);
        Assert.True(intersection3 == null);
        
        var ray4 = new Ray(new Point(0f, 0f, 1f), VecY);
        var intersection4 = plane.ray_intersection(ray4);
        Assert.True(intersection4 == null);
    }
    
    [Fact]
    public void TestUvCoordinates()
    {
        var plane = new Plane();

        var ray1 = new Ray(new Point(0.0f, 0.0f, 1.0f), VecZ.neg());
        var intersection1 = plane.ray_intersection(ray1);
        Assert.True(intersection1?.SurfacePoint.is_close(new Vec2d(0.0f, 0.0f)));
        
        var ray2 = new Ray(new Point(0.25f, 0.75f, 1.0f), VecZ.neg());
        var intersection2 = plane.ray_intersection(ray2);
        Assert.True(intersection2?.SurfacePoint.is_close(new Vec2d(0.25f, 0.75f)));
        
        var ray3 = new Ray(new Point(4.25f, 7.75f, 1.0f), VecZ.neg());
        var intersection3 = plane.ray_intersection(ray3);
        Assert.True(intersection3?.SurfacePoint.is_close(new Vec2d(0.25f, 0.75f)));

    }
}