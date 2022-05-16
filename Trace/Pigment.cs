using Trace.Geometry;

namespace Trace;

/// <summary>
/// This abstract class represents a pigment,
/// i.e., a function that associates a color with each point on a parametric surface (u,v).
/// Call the method :meth:`.Pigment.get_color` to retrieve the color of the surface given a :class:`.Vec2d` object.
/// </summary>
public interface IPigment
{
        
    /// <summary>
    /// Return the color of the pigment at the specified coordinates.
    /// </summary>
    public Color get_color(Vec2d uv);
    
}

/// <summary>
/// A uniform pigment. This is the most boring pigment: a uniform hue over the whole surface.
/// </summary>
public class UniformPigment : IPigment
{
    public Color Color { get; set; }

    public UniformPigment(Color color)
    {
        Color = color;
    }

    public Color get_color(Vec2d uv)
    {
        return Color;
    }
}

/// <summary>
/// A textured pigment. The texture is given through a PFM image.
/// </summary>
public class ImagePigment : IPigment
{
    public HdrImage Image { get; set; }

    public ImagePigment(HdrImage image)
    {
        Image = image;
    }

    public Color get_color(Vec2d uv)
    {
        int col = (int) (uv.U * Image.Width);
        int row = (int) (uv.V * Image.Height);

        if (col >= Image.Width)
        {
            col = Image.Width - 1;
        }

        if (row >= Image.Height)
        {
            row = Image.Height - 1;
        }

        return Image.get_pixel(col, row);

    }
}

/// <summary>
/// A checkered pigment. The number of rows/columns in the checkered pattern is tunable,
/// but you cannot have a different number of repetitions along the u/v directions
/// </summary>
public class CheckeredPigment : IPigment
{
    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public int NumOfSteps { get; set; }

    public CheckeredPigment(Color color1, Color color2, int numOfSteps = 10)
    {
        Color1 = color1;
        Color2 = color2;
        NumOfSteps = numOfSteps;
    }

    public Color get_color(Vec2d uv)
    {
        int int_u = (int) (Math.Floor(uv.U * NumOfSteps));
        int int_v = (int) (Math.Floor(uv.V * NumOfSteps));

        return int_u % 2 == int_v % 2 ? Color1 : Color2;
    }

}



