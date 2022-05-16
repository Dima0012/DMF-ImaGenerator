using Trace.Cameras;
using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class RendererTests
{
    private Color Black = new(0, 0, 0);
    private Color White = new(1, 1, 1);

    [Fact]
    public void TestOnOff()
    {
        var sphere = new Sphere(
            Transformation.translation(new Vec(2, 0, 0)) * Transformation.scaling(new Vec(0.2f, 0.2f, 0.2f)),
            new Material(new DiffuseBrdf(new UniformPigment(White))));

        var img = new HdrImage(3, 3);
        var cam = new OrthogonalCamera((float) 3 / 3, new Transformation());
        var imgTracer = new ImageTracer(img, cam);
        var world = new World();
        world.add(sphere);
        var renderer = new OnOffRenderer(world);
        imgTracer.fire_all_rays(renderer);

        Assert.True(img.get_pixel(0, 0).is_close(Black));
        Assert.True(img.get_pixel(1, 0).is_close(Black));
        Assert.True(img.get_pixel(2, 0).is_close(Black));

        Assert.True(img.get_pixel(0, 1).is_close(Black));
        Assert.True(img.get_pixel(1, 1).is_close(White));
        Assert.True(img.get_pixel(2, 1).is_close(Black));

        Assert.True(img.get_pixel(0, 2).is_close(Black));
        Assert.True(img.get_pixel(1, 2).is_close(Black));
        Assert.True(img.get_pixel(2, 2).is_close(Black));
    }

    [Fact]
    public void TestFlat()
    {
        var sphereColor = new Color(1, 2, 3);

        var sphere = new Sphere(
            Transformation.translation(new Vec(2, 0, 0)) * Transformation.scaling(new Vec(0.2f, 0.2f, 0.2f)),
            new Material(new DiffuseBrdf(new UniformPigment(sphereColor))));

        var img = new HdrImage(3, 3);
        var cam = new OrthogonalCamera((float) 3 / 3, new Transformation());
        var imgTracer = new ImageTracer(img, cam);
        var world = new World();
        world.add(sphere);
        var renderer = new FlatRenderer(world);
        imgTracer.fire_all_rays(renderer);

        Assert.True(img.get_pixel(0, 0).is_close(Black));
        Assert.True(img.get_pixel(1, 0).is_close(Black));
        Assert.True(img.get_pixel(2, 0).is_close(Black));

        Assert.True(img.get_pixel(0, 1).is_close(Black));
        Assert.True(img.get_pixel(1, 1).is_close(sphereColor));
        Assert.True(img.get_pixel(2, 1).is_close(Black));

        Assert.True(img.get_pixel(0, 2).is_close(Black));
        Assert.True(img.get_pixel(1, 2).is_close(Black));
        Assert.True(img.get_pixel(2, 2).is_close(Black));
    }
}