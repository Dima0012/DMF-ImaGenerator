namespace Trace.Geometry;

/// <summary>
/// A Point in 3D space.
/// </summary>
public struct Point
{
    public float X { get; set; } = 0.0f;
    public float Y { get; set; } = 0.0f;
    public float Z { get; set; } = 0.0f;

    public Point(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public string to_string()
    {
        return $"Point < x:{X}, y:{Y}, z:{Z} >";
    }
    
    /// <summary>
    /// Checks if two Vectors are equal within a epsilon threshold.
    /// </summary>
    public bool is_close(Point p, double epsilon = 1e-5)
    {
        return Math.Abs(X - p.X) < epsilon && Math.Abs(Y - p.Y) < epsilon && Math.Abs(Z - p.Z) < epsilon;
    }

    public static Point operator +(Point p, Vec v)
    {
        return new Point(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
    }
    
    /// <summary>
    /// Returns a Vector as the difference between two Points.
    /// </summary>
    public static Vec operator -(Point p, Point q)
    {
        return new Vec(p.X - q.X, p.Y - q.Y, p.Z - q.Z);
    }

    /// <summary>
    /// Returns a Point as the difference between a Point and a Vector. 
    /// </summary>
    public static Point operator -(Point p, Vec v)
    {
        return new Point(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
    }
    
    /// <summary>
    /// Overloading for product of a Point and a scalar. 
    /// </summary>
    public static Point operator *(Point p, float alpha)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    /// <summary>
    /// Overloading for product of a Point and a scalar. 
    /// </summary>
    public static Point operator *(float alpha, Point p)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    /// <summary>
    /// Converts the Point to a Vector.
    /// </summary>
    public Vec to_vec()
    {
        return new Vec(X, Y, Z);
    }


}