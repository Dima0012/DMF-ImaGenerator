namespace Trace;

/// <summary>
/// This class represents a HDR image as a matrix of Color elements, each one representing a pixel.
/// </summary>
public class HdrImage
{
    public int Height { get; set; }
    public int Width { get; set; }

    private Color[] Pixels;

    public HdrImage(int width, int height)
    {
        Height = height;
        Width = width;
        Pixels = new Color[Height * Width];

        for (int i = 0; i < Height * Width; i++)
        {
            Pixels[i] = new Color();
        }
    }

    public bool valid_coordinates(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    public int pixel_offset(int x, int y)
    {
        return y * Width + x;
    }
    
    public Color get_pixel(int x, int y)
    {
        System.Diagnostics.Trace.Assert(valid_coordinates(x, y));
        return Pixels[pixel_offset(x, y)];
    }

    public void set_pixel(int x, int y, Color color)
    {
        System.Diagnostics.Trace.Assert(valid_coordinates(x, y));
        Pixels[pixel_offset(x, y)] = color;
    }
}