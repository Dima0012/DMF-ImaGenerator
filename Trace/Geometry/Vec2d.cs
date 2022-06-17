namespace Trace.Geometry;

/// <summary>
///     A 2D vector used to represent a point on a surface
///     The fields are named u and v to distinguish them
///     from the usual 3D coordinates x, y, z.
/// </summary>
public struct Vec2d
{
    public float U { get; set; } = 0.0f;
    public float V { get; set; } = 0.0f;

    public Vec2d(float u, float v)
    {
        U = u;
        V = v;
    }

    public string to_string()
    {
        return $"Vec < u: {U}, v: {V}>";
    }

    /// <summary>
    ///     Checks if two Vectors2d are equal within a epsilon threshold.
    /// </summary>
    public bool is_close(Vec2d vec, double epsilon = 1e-5)
    {
        return Math.Abs(U - vec.U) < epsilon && Math.Abs(V - vec.V) < epsilon;
    }
}