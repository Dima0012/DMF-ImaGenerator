using System.ComponentModel;

namespace Trace;

public struct Color
{
    public float R { get; set; }
    public float G { get; set; }
    public float B { get; set; }

    public Color()
    {
        R = 0.0f;
        G = 0.0f;
        B = 0.0f;
    }

    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public static Color operator +(Color a, Color b)
    {
        Color newCol = new Color(a.R + b.R, a.G + b.G, a.B + b.B);
        return newCol;
    }

    public static Color operator -(Color a, Color b)
        // do we need absolute value?
    {
        Color newCol = new Color(a.R - b.R, a.G - b.G, a.B - b.B);
        return newCol;
    }

    public static Color operator *(Color a, Color b)
    {
        Color newCol = new Color(a.R * b.R, a.G * b.G, a.B * b.B);
        return newCol;
    }

    //vector- scalar product
    public static Color operator *(Color a, float alpha)
    {
        Color newCol = new Color(a.R * alpha, a.G * alpha, a.B * alpha);
        return newCol;
    }

    //scalar-vector product
    public static Color operator *(float alpha, Color a)
    {
        Color newCol = new Color(a.R * alpha, a.G * alpha, a.B * alpha);
        return newCol;
    }

    public bool is_close(Color a, double epsilon = 1e-5)
    {
        return Math.Abs(R - a.R) < epsilon && Math.Abs(G - a.G) < epsilon && Math.Abs(B - a.B) < epsilon;
    }

    public string to_string(Color a)
    {
        return "r: " + a.R + ", g: " + a.G + ", b: " + a.B;
    }
}