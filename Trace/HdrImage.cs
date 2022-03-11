namespace Trace;
/// <summary>
/// This class represents a HDR image as a matrix of element Color, each one representing a pixel.
/// </summary>
public class HdrImage
{
    private int Height { get; set; }
    private int Width { get; set; }

    private Color[] Pixels;

    private HdrImage(int height, int width)
    {
        Height = height;
        Width = width;
        Pixels = new Color[Height * Width];

        for (int i = 0; i < Height * Width; i++)
        {
            Pixels[i] = new Color();
        }
    }

    private bool valid_coordinates(int x, int y)
    {
        return x >= 0 && x < Width && y >= 0 && y < Height;
    }

    private int pixel_offset(int x, int y)
    {
        return y * Width + x;
    }
    
    private Color GetPixel(int x, int y)
    {
        return Pixels[pixel_offset(x,y)];
    }

    private void SetPixel(int x, int y, Color color)
    {
        Pixels[pixel_offset(x,y)] = color;
    }
}