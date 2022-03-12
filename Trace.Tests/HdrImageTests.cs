using Xunit;

namespace Trace.Tests;

public class HdrImageTests
{
    [Fact]
    public void TestImageCreation()
    {
        HdrImage img = new HdrImage(7, 4);
        Assert.True(7 == img.Width);
        Assert.True(4 == img.Height);
    }

    [Fact]
    public void TestCoordinates()
    {
        HdrImage img = new HdrImage(7, 4);

        Assert.True(img.valid_coordinates(0, 0));
        Assert.True(img.valid_coordinates(6, 3));
        Assert.False(img.valid_coordinates(-1, 0));
        Assert.False(img.valid_coordinates(0, -1));
        Assert.False(img.valid_coordinates(7, 0));
        Assert.False(img.valid_coordinates(0, 4));
    }

    [Fact]
    public void TestPixelOffset()
    {
        HdrImage img = new HdrImage(7, 4);

        Assert.True(0 == img.pixel_offset(0, 0));
        Assert.True(17 == img.pixel_offset(3, 2));
        Assert.True(7 * 4 - 1 == img.pixel_offset(6, 3));
    }

    [Fact]
    public void TestPixelAccess()
    {
        HdrImage img = new HdrImage(7, 4);

        Color referenceColor = new Color(1.0f, 2.0f, 3.0f);
        img.SetPixel(3, 2, referenceColor);
        Assert.True(referenceColor.is_close(img.GetPixel(3, 2)));
    }
}