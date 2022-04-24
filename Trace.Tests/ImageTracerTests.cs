using Xunit;
using Trace.Cameras;
using Trace.Geometry;

namespace Trace.Tests;

public class ImageTracerTests
{
    [Fact]
    public void TestImageTracer()
    {
        var image = new HdrImage(4, 2);
        var cam = new PerspectiveCamera(2.0f, new Transformation());
        var tracer = new ImageTracer(image, cam);

        var ray1 = tracer.fire_ray(0, 0, 2.5f, 1.5f);
        var ray2 = tracer.fire_ray(2, 1, 0.5f, 0.5f);
        Assert.True(ray1.is_close(ray2));

        // tracer.fire_all_rays();
        tracer.fire_all_rays(_ => new Color(1f, 2f, 3f));
        for (int row = 0; row < image.Height; row++)
        {
            for (int col = 0; col < image.Width; col++)
            {
                // Assert.True(image.get_pixel(col, row).is_close(new Color(1f, 2f, 3f)));
                Assert.True(image.get_pixel(col, row).is_close(new Color(1f, 2f, 3f)));
            }
        }
    }
}