namespace Trace.Geometry;

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
    
    public bool is_close(Point p, double epsilon = 1e-5)
    {
        return Math.Abs(X - p.X) < epsilon && Math.Abs(Y - p.Y) < epsilon && Math.Abs(Z - p.Z) < epsilon;
    }

    public static Point operator +(Point p, Vec v)
    {
        return new Point(p.X + v.X, p.Y + v.Y, p.Z + v.Z);
    }

    public static Vec operator -(Point p, Point q)
    {
        return new Vec(p.X - q.X, p.Y - q.Y, p.Z - q.Z);
    }

    public static Point operator -(Point p, Vec v)
    {
        return new Point(p.X - v.X, p.Y - v.Y, p.Z - v.Z);
    }
    
    public static Point operator *(Point p, float alpha)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    public static Point operator *(float alpha, Point p)
    {
        return new Point(p.X * alpha, p.Y * alpha, p.Z * alpha);
    }

    public Vec to_vec()
    {
        return new Vec(X, Y, Z);
    }


}