namespace Trace.Geometry;

/// <summary>
/// A Vector in 3D space.
/// </summary>
public struct Vec
{
    public float X { get; set; } = 0.0f;
    public float Y { get; set; } = 0.0f;
    public float Z { get; set; } = 0.0f;

    public Vec(float x, float y, float z)
    {
        X = x;
        Y = y;
        Z = z;
    }


    public string to_string()
    {
        return $"Vec < x: {X}, y: {Y}, z: {Z} >";
    }

    /// <summary>
    /// Checks if two Vectors are equal within a epsilon threshold.
    /// </summary>
    public bool is_close(Vec v, double epsilon = 1e-5)
    {
        return Math.Abs(X - v.X) < epsilon && Math.Abs(Y - v.Y) < epsilon && Math.Abs(Z - v.Z) < epsilon;
    }

    public static Vec operator +(Vec v, Vec u)
    {
        return new Vec(v.X + u.X, v.Y + u.Y, v.Z + u.Z);
    }

    public static Vec operator -(Vec v, Vec u)
    {
        return new Vec(v.X - u.X, v.Y - u.Y, v.Z - u.Z);
    }

    /// <summary>
    /// Overloading for product of a Vector and a scalar. 
    /// </summary>
    public static Vec operator *(Vec v, float alpha)
    {
        return new Vec(v.X * alpha, v.Y * alpha, v.Z * alpha);
    }

    /// <summary>
    /// Overloading for product of a Vector and a scalar. 
    /// </summary>
    public static Vec operator *(float alpha, Vec v)
    {
        return new Vec(v.X * alpha, v.Y * alpha, v.Z * alpha);
    }

    /// <summary>
    /// Returns the negation of the Vector -v .
    /// </summary>
    public Vec neg()
    {
        return new Vec(-X, -Y, -Z);
    }

    /// <summary>
    /// Overloading for the scalar product of two Vectors.
    /// </summary>
    public static float operator *(Vec v, Vec u)
    {
        return v.X * u.X + v.Y * u.Y + v.Z * u.Z;
    }

    /// <summary>
    /// Returns the vector product of the Vector with the Vector v.
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
    /// Returns the norm of the Vector.
    /// </summary>
    public float norm()
    {
        return MathF.Sqrt(squared_norm());
    }

    /// <summary>
    /// Returns the squared norm of the Vector.
    /// </summary>
    public float squared_norm()
    {
        return X * X + Y * Y + Z * Z;
    }

    /// <summary>
    /// Normalize the Vector by invoking norm().
    /// </summary>
    public Vec normalize()
    {
        X /= (float) norm();
        Y /= (float) norm();
        Z /= (float) norm();
        return this;
    }

    public Normal to_normal()
    {
        return new Normal(X, Y, Z);
    }
}