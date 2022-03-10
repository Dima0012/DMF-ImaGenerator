using System.ComponentModel;

namespace Trace;

public struct Color
{
    private float R { get; set; }
    private float G { get; set; }
    private float B { get; set; }

    public Color(float r, float g, float b)
    {
        R = r;
        G = g;
        B = b;
    }

    public Color Add(Color col)
    {
        Color newCol = new Color(R + col.R, G + col.G, B + col.B);
        return newCol;
    }

    public Color Diff(Color col)
        // do we need absolute value?
    {
        Color newCol = new Color(R - col.R, G - col.G, B - col.B);
        return newCol;
    }

    public Color Mul(Color col)
    {
        Color newCol = new Color(R * col.R, G * col.G, B * col.B);
        return newCol;
    }

    public Color Scalar_Mul(float alpha)
    {
        Color newCol = new Color(R * alpha, G * alpha, B * alpha);
        return newCol;
    }

    public bool are_close(double x, double y,
            double epsilon = 1e-5) //should we put this function outside both the struct Color and the class HdrImage?
    {
        return Math.Abs(x - y) < epsilon;
    }

    public bool are_colors_close(Color a, Color b)
    {
        return are_close(a.R, b.R) && are_close(a.G, b.G) && are_close(a.B, b.B);
    }
}
