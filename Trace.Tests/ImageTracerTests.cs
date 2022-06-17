using Trace.Cameras;
using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class ImageTracerTests
{
    public PerspectiveCamera cam;
    public HdrImage image;
    public ImageTracer tracer;

    public ImageTracerTests()
    {
        image = new HdrImage(4, 2);
        cam = new PerspectiveCamera(2.0f, new Transformation());
        tracer = new ImageTracer(image, cam);
    }

    [Fact]
    public void TestUvSubMapping()
    {
        var ray1 = tracer.fire_ray(0, 0, 2.5f, 1.5f);
        var ray2 = tracer.fire_ray(2, 1);
        Assert.True(ray1.is_close(ray2));
    }

    [Fact]
    public void TestImageCoverage()
    {
        var world = new World();
        tracer.fire_all_rays(new OneColor(world));
        for (var row = 0; row < image.Height; row++)
        for (var col = 0; col < image.Width; col++)
            Assert.True(image.get_pixel(col, row).is_close(new Color(1f, 2f, 3f)));
    }

    [Fact]
    public void TestOrientation()
    {
        var topLeftRay = tracer.fire_ray(0, 0, 0.0f, 0.0f);
        Assert.True(new Point(0f, 2f, 1f).is_close(topLeftRay.at(1f)));

        var bottomRightRay = tracer.fire_ray(3, 1, 1f, 1f);
        Assert.True(new Point(0f, -2f, -1f).is_close(bottomRightRay.at(1f)));
    }

    [Fact]
    public void TestAntiAliasing()
    {
        var hdrImage = new HdrImage(1, 1);
        var camera = new OrthogonalCamera(1, new Transformation());
        var imageTracer = new ImageTracer(hdrImage, camera, 10, new Pcg());

        var world = new World();

        var aliasingRenderer = new AliasingRenderer(world);

        imageTracer.fire_all_rays(aliasingRenderer);

        //Check that the number of rays that were fired is what we expect (100) and every one has tests passed.
        Assert.True(aliasingRenderer.Test1 == 100 && aliasingRenderer.Test2 == 100 && aliasingRenderer.Test3 == 100);
        Assert.True(aliasingRenderer.NumOfRays == 100);
    }
}