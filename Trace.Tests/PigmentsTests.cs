using Trace.Geometry;
using Xunit;

namespace Trace.Tests;

public class PigmentsTests
{
    [Fact]
    public void TestUniformPigment()
    {
        var color = new Color(1, 2, 3);
        var pigment = new UniformPigment(color);

        Assert.True(pigment.get_color(new Vec2d(0.0f, 0.0f)).is_close(color));
        Assert.True(pigment.get_color(new Vec2d(1.0f, 0.0f)).is_close(color));
        Assert.True(pigment.get_color(new Vec2d(0.0f, 1.0f)).is_close(color));
        Assert.True(pigment.get_color(new Vec2d(1.0f, 1.0f)).is_close(color));
    }

    [Fact]
    public void TestImagePigment()
    {
        var image = new HdrImage(2, 2);
        image.set_pixel(0, 0, new Color(1.0f, 2.0f, 3.0f));
        image.set_pixel(1, 0, new Color(2.0f, 3.0f, 1.0f));
        image.set_pixel(0, 1, new Color(2.0f, 1.0f, 3.0f));
        image.set_pixel(1, 1, new Color(3.0f, 2.0f, 1.0f));

        var pigment = new ImagePigment(image);
        Assert.True(pigment.get_color(new Vec2d(0.0f, 0.0f)).is_close(new Color(1.0f, 2.0f, 3.0f)));
        Assert.True(pigment.get_color(new Vec2d(1.0f, 0.0f)).is_close(new Color(2.0f, 3.0f, 1.0f)));
        Assert.True(pigment.get_color(new Vec2d(0.0f, 1.0f)).is_close(new Color(2.0f, 1.0f, 3.0f)));
        Assert.True(pigment.get_color(new Vec2d(1.0f, 1.0f)).is_close(new Color(3.0f, 2.0f, 1.0f)));
    }

    [Fact]
    public void TestCheckeredPigment()
    {
        var color1 = new Color(1.0f, 2.0f, 3.0f);
        var color2 = new Color(10.0f, 20.0f, 30.0f);

        var pigment = new CheckeredPigment(color1, color2, 2);

        // With num_of_steps == 2, the pattern should be the following:

        //              (0.5, 0)
        //   (0, 0) +------+------+ (1, 0)
        //          |      |      |
        //          | col1 | col2 |
        //          |      |      |
        // (0, 0.5) +------+------+ (1, 0.5)
        //          |      |      |
        //          | col2 | col1 |
        //          |      |      |
        //   (0, 1) +------+------+ (1, 1)
        //              (0.5, 1)

        Assert.True(pigment.get_color(new Vec2d(0.25f, 0.25f)).is_close(color1));
        Assert.True(pigment.get_color(new Vec2d(0.75f, 0.25f)).is_close(color2));
        Assert.True(pigment.get_color(new Vec2d(0.25f, 0.75f)).is_close(color2));
        Assert.True(pigment.get_color(new Vec2d(0.75f, 0.75f)).is_close(color1));
    }
}