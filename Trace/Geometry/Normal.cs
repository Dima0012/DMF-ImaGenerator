namespace Trace.Geometry;

public struct Normal
{
    public float X { get; set; } = 0.0f;
    public float Y { get; set; } = 0.0f;
    public float Z { get; set; } = 0.0f;
    
    public Normal(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }

    public string to_string()
    {
        return $"Normal < x: {X}, y: {Y}, z: {Z} >";
    }

    public bool is_close(Normal n, double epsilon=10-5)
    {
        return Math.Abs(X - n.X) < epsilon && Math.Abs(Y - n.Y) < epsilon && Math.Abs(Z - n.Z) < epsilon;
    }

    public Normal neg()
    {
        return new Normal(-X, -Y, -Z);
    }

    public static Normal operator *(float a, Normal n)
    {
        return new Normal(a * n.X, a * n.Y, a * n.Z);
    }
    
    public static Normal operator *(Normal n, float a)
    {
        return new Normal(a * n.X, a * n.Y, a * n.Z);
    }

    public static double operator *(Normal n, Normal m)
    {
        return n.X * m.X + n.Y * m.Y + n.Z * m.Z;
    }
    
    public Vec vec_prod(Vec v)
    {
        return new Vec(
            Y * v.Z - Z * v.Y,
            Z * v.X - X * v.Z,
            X * v.Y - Y * v.X
        );
    }
    
    public Vec vec_prod(Normal n)
    {
        return new Vec(
            Y * n.Z - Z * n.Y,
            Z * n.X - X * n.Z,
            X * n.Y - Y * n.X
        );
    }
    
    public double norm()
    {
        return Math.Sqrt(squared_norm());
    }

    public double squared_norm()
    {
        return X * X + Y * Y + Z * Z;
    }

    public void normalize()
    {
        X /= (float) norm();
        Y /= (float) norm();
        Z /= (float) norm();
    }

}