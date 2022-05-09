namespace Trace.Geometry;

/// <summary>
/// A Normal Vector in 3D space.
/// </summary>
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

    /// <summary>
    /// Checks if two Normals are equal within a epsilon threshold.
    /// </summary>
    public bool is_close(Normal n, double epsilon=10-5)
    {
        return Math.Abs(X - n.X) < epsilon && Math.Abs(Y - n.Y) < epsilon && Math.Abs(Z - n.Z) < epsilon;
    }

    /// <summary>
    /// Returns the negation of the Normal -n .
    /// </summary>
    public Normal neg()
    {
        return new Normal(-X, -Y, -Z);
    }

    /// <summary>
    /// Overloading for product of a Normal and a scalar. 
    /// </summary>
    public static Normal operator *(float a, Normal n)
    {
        return new Normal(a * n.X, a * n.Y, a * n.Z);
    }
    
    /// <summary>
    /// Overloading for product of a Normal and a scalar. 
    /// </summary>
    public static Normal operator *(Normal n, float a)
    {
        return new Normal(a * n.X, a * n.Y, a * n.Z);
    }

    /// <summary>
    /// Overloading for the scalar product of two Vectors.
    /// </summary>
    public static double operator *(Normal n, Normal m)
    {
        return n.X * m.X + n.Y * m.Y + n.Z * m.Z;
    }
    
    /// <summary>
    /// Returns the vector product of the Normal with the Vector v as a Vector.
    /// </summary>
    public Vec vec_prod(Vec v)
    {
        return new Vec(
            Y * v.Z - Z * v.Y,
            Z * v.X - X * v.Z,
            X * v.Y - Y * v.X
        );
    }
    
    /// <summary>
    /// Returns the vector product of the Normal with the Normal n as a Vector.
    /// </summary>
    public Vec vec_prod(Normal n)
    {
        return new Vec(
            Y * n.Z - Z * n.Y,
            Z * n.X - X * n.Z,
            X * n.Y - Y * n.X
        );
    }
    
    /// <summary>
    /// Returns the norm of the Normal.
    /// </summary>
    public double norm()
    {
        return Math.Sqrt(squared_norm());
    }

    /// <summary>
    /// Returns the norm of the Normal.
    /// </summary>
    public double squared_norm()
    {
        return X * X + Y * Y + Z * Z;
    }

    /// <summary>
    /// Normalize the Normal by invoking norm().
    /// </summary>
    public Vec normalize()
    {
        return new Vec(X/(float) norm(), Y/(float) norm(), Z/(float) norm());
    }

}