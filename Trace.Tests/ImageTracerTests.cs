using Xunit;
using Trace.Cameras;
using Trace.Geometry;

namespace Trace.Tests;

public class ImageTracerTests
{
    public HdrImage image;
    public PerspectiveCamera cam;
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
        var ray2 = tracer.fire_ray(2, 1, 0.5f, 0.5f);
        Assert.True(ray1.is_close(ray2));
    }

    [Fact]
    public void TestImageCoverage()
    {
        tracer.fire_all_rays(_ => new Color(1f, 2f, 3f));
        for (int row = 0; row < image.Height; row++)
        {
            for (int col = 0; col < image.Width; col++)
            {
                Assert.True(image.get_pixel(col, row).is_close(new Color(1f, 2f, 3f)));
            }
        }
    }

    [Fact]
    public void TestOrientation()
    {
        var topLeftRay = tracer.fire_ray(0,0, 0.0f, 0.0f);
        Assert.True(new Point(0f,2f,1f).is_close(topLeftRay.at(1f)));

        var bottomRightRay = tracer.fire_ray(3, 1, 1f, 1f);
        Assert.True(new Point(0f,-2f,-1f).is_close(bottomRightRay.at(1f)));
    }
}