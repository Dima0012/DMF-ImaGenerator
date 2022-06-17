using System.IO;
using System.Text;
using Xunit;

namespace Trace.Tests;

public class HdrImageTests
{
    [Fact]
    public void TestImageCreation()
    {
        var img = new HdrImage(7, 4);
        Assert.True(7 == img.Width);
        Assert.True(4 == img.Height);
    }

    [Fact]
    public void TestCoordinates()
    {
        var img = new HdrImage(7, 4);

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
        var img = new HdrImage(7, 4);

        Assert.True(0 == img.pixel_offset(0, 0));
        Assert.True(17 == img.pixel_offset(3, 2));
        Assert.True(7 * 4 - 1 == img.pixel_offset(6, 3));
    }

    [Fact]
    public void TestPixelAccess()
    {
        var img = new HdrImage(7, 4);

        var referenceColor = new Color(1.0f, 2.0f, 3.0f);
        img.set_pixel(3, 2, referenceColor);
        Assert.True(referenceColor.is_close(img.get_pixel(3, 2)));
    }

    [Fact]
    public void TestWritePfm()
    {
        var img = new HdrImage(3, 2);

        img.set_pixel(0, 0, new Color(1.0e1f, 2.0e1f, 3.0e1f));
        img.set_pixel(1, 0, new Color(4.0e1f, 5.0e1f, 6.0e1f));
        img.set_pixel(2, 0, new Color(7.0e1f, 8.0e1f, 9.0e1f));
        img.set_pixel(0, 1, new Color(1.0e2f, 2.0e2f, 3.0e2f));
        img.set_pixel(1, 1, new Color(4.0e2f, 5.0e2f, 6.0e2f));
        img.set_pixel(2, 1, new Color(7.0e2f, 8.0e2f, 9.0e2f));

        //Little-endian format 
        byte[] referenceBytesLe =
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
        };

        // Writing img on memory stream
        using var memStream = new MemoryStream();
        img.write_pfm(memStream, -1.0);
        var buf = memStream.GetBuffer();

        for (var i = 0; i < referenceBytesLe.Length; i++) Assert.True(referenceBytesLe[i] == buf[i]);

        //Big-endian format 
        byte[] referenceBytesBe =
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x31, 0x2e, 0x30, 0x0a, 0x42,
            0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43,
            0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44,
            0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00, 0x41,
            0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 0x42,
            0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 0x42,
            0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
        };

        // Writing img on memory stream
        using var memStream_B = new MemoryStream();
        img.write_pfm(memStream_B, 1.0);
        var buf_B = memStream_B.GetBuffer();

        for (var i = 0; i < referenceBytesBe.Length; i++) Assert.True(referenceBytesBe[i] == buf_B[i]);
    }

    [Fact]
    public void TestReadLine()
    {
        var img = new HdrImage(0, 0);

        var buf = Encoding.ASCII.GetBytes("hello\nworld");
        using var memStream = new MemoryStream(buf);

        Assert.True(img.read_line(memStream) == "hello");
        Assert.True(img.read_line(memStream) == "world");
        Assert.True(img.read_line(memStream) == "");
    }

    [Fact]
    public void TestParseEndianness()
    {
        var img = new HdrImage(0, 0);

        Assert.True(false == img.parse_endianness("1.0"));
        Assert.True(img.parse_endianness("-1.0"));

        // Throws exception (via lambda expression) and check if are raised correctly
        Assert.Throws<InvalidPfmFileFormat>(() => img.parse_endianness("0.0"));
        Assert.Throws<InvalidPfmFileFormat>(() => img.parse_endianness("abc"));
    }

    [Fact]
    public void TestParseImgSize()
    {
        var img = new HdrImage(0, 0);

        Assert.True(img.parse_img_size("3 2") == (3, 2));

        // Throws exception (via lambda expression) and check if are raised correctly
        Assert.Throws<InvalidPfmFileFormat>(() => img.parse_img_size("-1 3"));
        Assert.Throws<InvalidPfmFileFormat>(() => img.parse_img_size("3 2 1"));
    }

    [Fact]
    public void TestReadPfm()
    {
        //Little-endian format 
        byte[] referenceBytesLe =
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x2d, 0x31, 0x2e, 0x30, 0x0a,
            0x00, 0x00, 0xc8, 0x42, 0x00, 0x00, 0x48, 0x43, 0x00, 0x00, 0x96, 0x43,
            0x00, 0x00, 0xc8, 0x43, 0x00, 0x00, 0xfa, 0x43, 0x00, 0x00, 0x16, 0x44,
            0x00, 0x00, 0x2f, 0x44, 0x00, 0x00, 0x48, 0x44, 0x00, 0x00, 0x61, 0x44,
            0x00, 0x00, 0x20, 0x41, 0x00, 0x00, 0xa0, 0x41, 0x00, 0x00, 0xf0, 0x41,
            0x00, 0x00, 0x20, 0x42, 0x00, 0x00, 0x48, 0x42, 0x00, 0x00, 0x70, 0x42,
            0x00, 0x00, 0x8c, 0x42, 0x00, 0x00, 0xa0, 0x42, 0x00, 0x00, 0xb4, 0x42
        };

        using var memStreamLe = new MemoryStream(referenceBytesLe);
        var imgLe = new HdrImage(memStreamLe);

        Assert.True(3 == imgLe.Width);
        Assert.True(2 == imgLe.Height);

        Assert.True(imgLe.get_pixel(0, 0).is_close(new Color(1.0e1f, 2.0e1f, 3.0e1f)));
        Assert.True(imgLe.get_pixel(1, 0).is_close(new Color(4.0e1f, 5.0e1f, 6.0e1f)));
        Assert.True(imgLe.get_pixel(2, 0).is_close(new Color(7.0e1f, 8.0e1f, 9.0e1f)));
        Assert.True(imgLe.get_pixel(0, 1).is_close(new Color(1.0e2f, 2.0e2f, 3.0e2f)));
        Assert.True(imgLe.get_pixel(0, 0).is_close(new Color(1.0e1f, 2.0e1f, 3.0e1f)));
        Assert.True(imgLe.get_pixel(1, 1).is_close(new Color(4.0e2f, 5.0e2f, 6.0e2f)));
        Assert.True(imgLe.get_pixel(2, 1).is_close(new Color(7.0e2f, 8.0e2f, 9.0e2f)));


        //Big-endian format 
        byte[] referenceBytesBe =
        {
            0x50, 0x46, 0x0a, 0x33, 0x20, 0x32, 0x0a, 0x31, 0x2e, 0x30, 0x0a, 0x42,
            0xc8, 0x00, 0x00, 0x43, 0x48, 0x00, 0x00, 0x43, 0x96, 0x00, 0x00, 0x43,
            0xc8, 0x00, 0x00, 0x43, 0xfa, 0x00, 0x00, 0x44, 0x16, 0x00, 0x00, 0x44,
            0x2f, 0x00, 0x00, 0x44, 0x48, 0x00, 0x00, 0x44, 0x61, 0x00, 0x00, 0x41,
            0x20, 0x00, 0x00, 0x41, 0xa0, 0x00, 0x00, 0x41, 0xf0, 0x00, 0x00, 0x42,
            0x20, 0x00, 0x00, 0x42, 0x48, 0x00, 0x00, 0x42, 0x70, 0x00, 0x00, 0x42,
            0x8c, 0x00, 0x00, 0x42, 0xa0, 0x00, 0x00, 0x42, 0xb4, 0x00, 0x00
        };

        using var memStreamBe = new MemoryStream(referenceBytesLe);
        var imgBe = new HdrImage(memStreamBe);

        Assert.True(3 == imgBe.Width);
        Assert.True(2 == imgBe.Height);

        Assert.True(imgBe.get_pixel(0, 0).is_close(new Color(1.0e1f, 2.0e1f, 3.0e1f)));
        Assert.True(imgBe.get_pixel(1, 0).is_close(new Color(4.0e1f, 5.0e1f, 6.0e1f)));
        Assert.True(imgBe.get_pixel(2, 0).is_close(new Color(7.0e1f, 8.0e1f, 9.0e1f)));
        Assert.True(imgBe.get_pixel(0, 1).is_close(new Color(1.0e2f, 2.0e2f, 3.0e2f)));
        Assert.True(imgBe.get_pixel(0, 0).is_close(new Color(1.0e1f, 2.0e1f, 3.0e1f)));
        Assert.True(imgBe.get_pixel(1, 1).is_close(new Color(4.0e2f, 5.0e2f, 6.0e2f)));
        Assert.True(imgBe.get_pixel(2, 1).is_close(new Color(7.0e2f, 8.0e2f, 9.0e2f)));
    }

    [Fact]
    public void TestReadPfmWrong()
    {
        var img = new HdrImage(0, 0);
        var buf = Encoding.ASCII.GetBytes("PF\n3 2\n-1.0\nstop");
        using var memStream = new MemoryStream(buf);

        // Check if a wrong PFM raises exceptions correctly.
        Assert.Throws<InvalidPfmFileFormat>(() => img.read_pfm_image(memStream));
    }

    [Fact]
    public void TestAverageLuminosity()
    {
        var img = new HdrImage(2, 1);
        img.set_pixel(0, 0, new Color(5.0f, 10.0f, 15.0f));
        img.set_pixel(1, 0, new Color(500.0f, 1000.0f, 1500.0f));
        Assert.True(img.double_is_close(100.0, img.average_luminosity(0.0)));
    }

    [Fact]
    public void TestDoubleClose()
    {
        var img = new HdrImage(2, 1);
        var a = 2.0;
        Assert.True(img.double_is_close(a, 2.0));
        Assert.False(img.double_is_close(a, 4.0));
    }

    [Fact]
    public void TestNormalizeImage()
    {
        var img = new HdrImage(2, 1);
        img.set_pixel(0, 0, new Color(5.0f, 10.0f, 15.0f));
        img.set_pixel(1, 0, new Color(500.0f, 1000.0f, 1500.0f));

        img.normalize_image(1000.0f, 100.0f);
        Assert.True(img.get_pixel(0, 0).is_close(new Color(0.5e2f, 1.0e2f, 1.5e2f)));
        Assert.True(img.get_pixel(1, 0).is_close(new Color(0.5e4f, 1.0e4f, 1.5e4f)));
    }

    [Fact]
    public void TestClampImage()
    {
        var img = new HdrImage(2, 1);
        img.set_pixel(0, 0, new Color(5.0e1f, 1.0e1f, 1.5e1f));
        img.set_pixel(1, 0, new Color(5.0e3f, 1.0e3f, 1.5e3f));

        img.clamp_image();

        //Just check that the R/G/B values are within the expected boundaries
        foreach (var curPixel in img.Pixels)
        {
            Assert.True(curPixel.R is >= 0 and <= 1);
            Assert.True(curPixel.G is >= 0 and <= 1);
            Assert.True(curPixel.B is >= 0 and <= 1);
        }
    }
}