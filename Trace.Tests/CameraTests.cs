using Trace.Cameras;
using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class CameraTests
{
    [Fact]
    public void TestOrthogonalCamera()
    {
        var cam = new OrthogonalCamera(2.0f, new Transformation());

        var ray1 = cam.fire_ray(0.0f, 0.0f);
        var ray2 = cam.fire_ray(1.0f, 0.0f);
        var ray3 = cam.fire_ray(0.0f, 1.0f);
        var ray4 = cam.fire_ray(1.0f, 1.0f);

        // Verify rays are parallel by checking vector product
        Assert.True(0.0f == ray1.Dir.vec_prod(ray2.Dir).squared_norm());
        Assert.True(0.0f == ray1.Dir.vec_prod(ray3.Dir).squared_norm());
        Assert.True(0.0f == ray1.Dir.vec_prod(ray4.Dir).squared_norm());

        // Verify rays are hitting corners at right coordinates
        Assert.True(ray1.at(1.0f).is_close(new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(ray2.at(1.0f).is_close(new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(ray3.at(1.0f).is_close(new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(ray4.at(1.0f).is_close(new Point(0.0f, -2.0f, 1.0f)));
    }

    [Fact]
    public void TestOrthogonalCameraTransform()
    {
        var translation = Transformation.translation(-2.0f * new Vec(0.0f, 1.0f, 0.0f));
        var rotation = Transformation.rotation_z(90);

        var transformation = translation * rotation;

        var cam = new OrthogonalCamera(transformation);

        var ray = cam.fire_ray(0.5f, 0.5f);
        Assert.True(ray.at(1.0f).is_close(new Point(0.0f, -2.0f, 0.0f)));
    }

    [Fact]
    public void TestPerspectiveCamera()
    {
        var cam = new PerspectiveCamera(1.0f, 2.0f, new Transformation());

        var ray1 = cam.fire_ray(0.0f, 0.0f);
        var ray2 = cam.fire_ray(1.0f, 0.0f);
        var ray3 = cam.fire_ray(0.0f, 1.0f);
        var ray4 = cam.fire_ray(1.0f, 1.0f);

        // Verify all rays depart from the origin
        Assert.True(ray1.Origin.is_close(ray2.Origin));
        Assert.True(ray1.Origin.is_close(ray3.Origin));
        Assert.True(ray1.Origin.is_close(ray4.Origin));

        // Verify the ray hitting the corners has right coordinates
        Assert.True(ray1.at(1.0f).is_close(new Point(0.0f, 2.0f, -1.0f)));
        Assert.True(ray2.at(1.0f).is_close(new Point(0.0f, -2.0f, -1.0f)));
        Assert.True(ray3.at(1.0f).is_close(new Point(0.0f, 2.0f, 1.0f)));
        Assert.True(ray4.at(1.0f).is_close(new Point(0.0f, -2.0f, 1.0f)));
    }
}