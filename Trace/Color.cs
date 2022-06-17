namespace Trace;

/// <summary>
///     A class to memorize a Color in RGB format.
/// </summary>
public struct Color
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }

    /// <summary>
    ///     Initialize a new instance of Color, with every component set to zero.
    /// </summary>
    public Color()
    {
        R = 0.0f;
        G = 0.0f;
        B = 0.0f;
    }

    /// <summary>
    ///     Initialize a new instance of Color, with the specified components.
    /// </summary>
    /// <param name="r"> The red component.</param>
    /// <param name="g"> The green component.</param>
    /// <param name="b"> The blue component. </param>
    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    // Operators overloading
    public static Color operator +(Color a, Color b)
    {
        var newCol = new Color(a.R + b.R, a.G + b.G, a.B + b.B);
        return newCol;
    }

    public static Color operator -(Color a, Color b)
        // do we need absolute value?
    {
        var newCol = new Color(a.R - b.R, a.G - b.G, a.B - b.B);
        return newCol;
    }

    public static Color operator *(Color a, Color b)
    {
        var newCol = new Color(a.R * b.R, a.G * b.G, a.B * b.B);
        return newCol;
    }

    //vector- scalar product
    public static Color operator *(Color a, float alpha)
    {
        var newCol = new Color(a.R * alpha, a.G * alpha, a.B * alpha);
        return newCol;
    }

    //scalar-vector product
    public static Color operator *(float alpha, Color a)
    {
        var newCol = new Color(a.R * alpha, a.G * alpha, a.B * alpha);
        return newCol;
    }

    public bool is_close(Color a, double epsilon = 1e-5)
    {
        return Math.Abs(R - a.R) < epsilon && Math.Abs(G - a.G) < epsilon && Math.Abs(B - a.B) < epsilon;
    }

    public bool float_is_close(float a, float b, double epsilon = 1e-5)
    {
        return Math.Abs(a - b) < epsilon;
    }

    public string to_string()
    {
        return $"< r: {R}, g: {G}, b: {B} >";
    }

    /// <summary>
    ///     Compute te luminosity of a Color object, as the half sum of the min and max component.
    /// </summary>
    public float luminosity()
    {
        return (Math.Max(R, Math.Max(G, B)) + Math.Min(R, Math.Min(G, B))) / 2;
    }
}