using Trace.Geometry;

namespace Trace;

/// <summary>
/// This abstract class represents a pigment,
/// i.e., a function that associates a color with each point on a parametric surface (u,v).
/// Call the method get_color to retrieve the color of the surface given a Vec2D object.
/// </summary>
public interface IPigment
{
        
    /// <summary>
    /// Return the color of the pigment at the specified coordinates.
    /// </summary>
    public Color get_color(Vec2d uv);
    
    //In order to be able to access these members from a IPigment object
    public Color Color { get; set; }
    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public int NumOfSteps { get; set; }
    
    
}

/// <summary>
/// A uniform pigment. This is the most boring pigment: a uniform hue over the whole surface.
/// </summary>
public class UniformPigment : IPigment
{
    public Color Color1 { get; set; }//
    public Color Color2 { get; set; }//
    public int NumOfSteps { get; set; }//useless, we need it because in this way it can me a member of
                                        //IPigment and we can access it when we have IPigment object
    
    public Color Color { get; set; }

    /// <summary>
    /// Create a white uniform Pigment.
    /// </summary>
    public UniformPigment()
    {
        Color = new Color(1, 1, 1);
    }

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
    
    
    public Color Color { get; set; }//
    public Color Color1 { get; set; }//useless, we need it because in this way it can me a member of
    public Color Color2 { get; set; }//IPigment and we can access it when we have IPigment object
    public int NumOfSteps { get; set; }//
    public HdrImage Image { get; set; }

    public ImagePigment(HdrImage image)
    {
        Image = image;
    }

    public Color get_color(Vec2d uv)
    {
        var col = (int) (uv.U * Image.Width);
        var row = (int) (uv.V * Image.Height);

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
    public Color Color { get; set; } //useless, we need it because in this way it can me a member of
                                     //IPigment and we can access it when we have IPigment object
    public Color Color1 { get; set; }
    public Color Color2 { get; set; }
    public int NumOfSteps { get; set; }

    public CheckeredPigment(int numOfSteps = 10)
    {
        // Green
        Color1 = new Color(0, 0.5f, 0.2f);
        // Blue
        Color2 = new Color(0, 0.2f, 0.5f);
        NumOfSteps = numOfSteps;
    }

    public CheckeredPigment(Color color1, Color color2, int numOfSteps = 10)
    {
        Color1 = color1;
        Color2 = color2;
        NumOfSteps = numOfSteps;
    }

    public Color get_color(Vec2d uv)
    {
        var intU = (int) (Math.Floor(uv.U * NumOfSteps));
        var intV = (int) (Math.Floor(uv.V * NumOfSteps));

        return intU % 2 == intV % 2 ? Color1 : Color2;
    }

}



