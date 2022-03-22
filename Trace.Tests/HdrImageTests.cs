using System;
using System.IO;
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
        img.set_pixel(3, 2, referenceColor);
        Assert.True(referenceColor.is_close(img.get_pixel(3, 2)));
    }

    [Fact]
    public void TestWrite_pfm()
    {
        HdrImage img = new HdrImage(3, 2);

        img.set_pixel(0, 0, new Color(1.0e1f, 2.0e1f, 3.0e1f));
        img.set_pixel(1, 0, new Color(4.0e1f, 5.0e1f, 6.0e1f));
        img.set_pixel(2, 0, new Color(7.0e1f, 8.0e1f, 9.0e1f));
        img.set_pixel(0, 1, new Color(1.0e2f, 2.0e2f, 3.0e2f));
        img.set_pixel(1, 1, new Color(4.0e2f, 5.0e2f, 6.0e2f));
        img.set_pixel(2, 1, new Color(7.0e2f, 8.0e2f, 9.0e2f));

        //Little-endian format 
        byte[] referenceBytes =
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

        for (var i = 0; i < referenceBytes.Length; i++)
        {
            Assert.True(referenceBytes[i] == buf[i]);
        }
        
        //Big-endian format 
        byte[] referenceBytes_B =
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

        for (var i = 0; i < referenceBytes_B.Length; i++)
        {
            Assert.True(referenceBytes_B[i] == buf_B[i]);
        }

    }
}