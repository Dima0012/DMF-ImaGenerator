using Xunit;
using Trace.Cameras;
using Trace.Geometry;

namespace Trace.Tests;

public class SphereTests
{
    private Vec VecX = new Vec(1f, 0f, 0f);
    private Vec VecY = new Vec(0f, 1f, 0f);
    private Vec VecZ = new Vec(0f, 0f, 1f);

    [Fact]
    public void TestHit()
    {
        var sphere = new Sphere();
        var ray1 = new Ray(new Point(0f, 0f, 2f), VecZ.neg());
        var intersection1 = sphere.ray_intersection(ray1);
        Assert.True(intersection1 != null);
        Assert.True(new HitRecord(
            new Point(0.0f, 0.0f, 1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            new Vec2d(0.0f, 0.0f),
            1.0f,
            ray1).is_close(intersection1));

        var ray2 = new Ray(new Point(3f, 0f, 0f), VecX.neg());
        var intersection2 = sphere.ray_intersection(ray2);
        Assert.True(intersection2 != null);
        Assert.True(new HitRecord(
            new Point(1.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            new Vec2d(0.0f, 0.5f),
            2.0f,
            ray2).is_close(intersection2));

        Assert.True(sphere.ray_intersection(new Ray(new Point(0f, 10f, 2f), VecZ.neg())) == null);
    }


    [Fact]
    public void TestInnerHit()
    {
        var sphere = new Sphere();
        var ray = new Ray(new Point(0f, 0f, 0f), VecX);
        var intersection = sphere.ray_intersection(ray);
        Assert.True(intersection != null);
        Assert.True(new HitRecord(
            new Point(1.0f, 0.0f, 0.0f),
            new Normal(-1.0f, 0.0f, 0.0f),
            new Vec2d(0.0f, 0.5f),
            1.0f,
            ray).is_close(intersection));
    }

    [Fact]
    public void TestTransformation()
    {
        var sphere = new Sphere(Transformation.translation(new Vec(10.0f, 0.0f, 0.0f)));

        var ray1 = new Ray(new Point(10f, 0f, 2f), VecZ.neg());
        var intersection1 = sphere.ray_intersection(ray1);
        Assert.True(intersection1 != null);
        Assert.True(new HitRecord(
            new Point(10.0f, 0.0f, 1.0f),
            new Normal(0.0f, 0.0f, 1.0f),
            new Vec2d(0.0f, 0.0f),
            1.0f,
            ray1).is_close(intersection1));

        var ray2 = new Ray(new Point(13f, 0f, 0f), VecX.neg());
        var intersection2 = sphere.ray_intersection(ray2);
        Assert.True(intersection2 != null);
        Assert.True(new HitRecord(
            new Point(11.0f, 0.0f, 0.0f),
            new Normal(1.0f, 0.0f, 0.0f),
            new Vec2d(0.0f, 0.5f),
            2.0f,
            ray2).is_close(intersection2));

        // Check if the sphere failed to move by trying to hit the untransformed shape
        Assert.True(sphere.ray_intersection(new Ray(new Point(0f, 0f, 2f), VecZ.neg())) == null);

        // Check if the *inverse* transformation was wrongly applied
        Assert.True(sphere.ray_intersection(new Ray(new Point(-10f, 0f, 0f), VecZ.neg())) == null);
    }

    [Fact]
    public void TestNormals()
    {
        var sphere = new Sphere(Transformation.scaling(new Vec(2.0f, 1.0f, 1.0f)));

        //The Vector here has zero third component because has direction normal to the Point(?)
        var ray = new Ray(new Point(1.0f, 1.0f, 0.0f), new Vec(-1.0f, -1.0f, 0.0f));

        var intersection = sphere.ray_intersection(ray);
        // We normalize "intersection.normal", as we are not interested in its length
        Assert.True(intersection != null &&
                    intersection.Normal.normalize().is_close(new Normal(1.0f, 4.0f, 0.0f).normalize()));
    }

    [Fact]
    public void TestNormalDirection()
    {
        //Scaling a sphere by -1 keeps the sphere the same but reverses its reference frame
        var sphere = new Sphere(Transformation.scaling(new Vec(-1.0f, -1.0f, -1.0f)));

        var ray = new Ray(new Point(0.0f, 2.0f, 0.0f), VecY.neg());
        var intersection = sphere.ray_intersection(ray);
        // We normalize "intersection.normal", as we are not interested in its length
        Assert.True(intersection != null &&
                    intersection.Normal.normalize().is_close(new Normal(0.0f, 1.0f, 0.0f).normalize()));
    }

    [Fact]
    public void TestUvCoordinates()
    {
        var sphere = new Sphere();

        /*The first four rays hit the unit sphere at the
        points P1, P2, P3, and P4.
               
                           ^ y
                           | P2
                     , - ~ * ~ - ,
                 , '       |       ' ,
               ,           |           ,
              ,            |            ,
             ,             |             , P1
        -----*-------------+-------------*---------> x
          P3 ,             |             ,
              ,            |            ,
               ,           |           ,
                 ,         |        , '
                   ' - , _ * _ ,  '
                           | P4
               
        P5 and P6 are aligned along the x axis and are displaced
        along z (ray5 in the positive direction, ray6 in the negative
        direction).*/

        var ray1 = new Ray(new Point(2.0f, 0.0f, 0.0f), VecX.neg());
        Assert.True(sphere.ray_intersection(ray1)!.SurfacePoint.is_close(new Vec2d(0.0f, 0.5f)));

        var ray2 = new Ray(new Point(0.0f, 2.0f, 0.0f), VecY.neg());
        Assert.True(sphere.ray_intersection(ray2)!.SurfacePoint.is_close(new Vec2d(0.25f, 0.5f)));

        var ray3 = new Ray(new Point(-2.0f, 0.0f, 0.0f), VecX);
        Assert.True(sphere.ray_intersection(ray3)!.SurfacePoint.is_close(new Vec2d(0.5f, 0.5f)));

        var ray4 = new Ray(new Point(0.0f, -2.0f, 0.0f), VecY);
        Assert.True(sphere.ray_intersection(ray4)!.SurfacePoint.is_close(new Vec2d(0.75f, 0.5f)));

        var ray5 = new Ray(new Point(2.0f, 0.0f, 0.5f), VecX.neg());
        Assert.True(sphere.ray_intersection(ray5)!.SurfacePoint.is_close(new Vec2d(0.0f, 1f / 3f)));

        var ray6 = new Ray(new Point(2.0f, 0.0f, -0.5f), VecX.neg());
        Assert.True(sphere.ray_intersection(ray6)!.SurfacePoint.is_close(new Vec2d(0.0f, 2f / 3f)));
    }
}