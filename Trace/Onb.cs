using Trace.Geometry;

namespace Trace;

/// <summary>
/// An orthonormal base in 3D space.
/// </summary>
public struct Onb
{
    public Vec E1 { get; set; } = new();
    public Vec E2 { get; set; } = new();
    public Vec E3 { get; set; } = new();

    /// <summary>
    /// Create a new ONB from Vec v.
    /// </summary>
    public Onb(Vec v)
    {
        CreateOnbFromZ(v);
    }

    /// <summary>
    /// Create a new ONB from Normal n.
    /// </summary>
    public Onb(Normal n)
    {
        CreateOnbFromZ(n);
    }

    /// <summary>
    /// Create a ONB from Z component of a Vec v using Duff et al. algorithm. The Vec v must be normalized.
    /// </summary>
    public void CreateOnbFromZ(Vec v)
    {
        var sign = MathF.CopySign(1.0f, v.Z);

        var a = -1.0f / (sign + v.Z);
        var b = v.X * v.Y * a;

        E1 = new Vec(1.0f + sign * v.X * v.X * a, sign * b, -sign * v.X);
        E2 = new Vec(b, sign + v.Y * v.Y * a, -v.Y);
        E3 = v;
    }

    /// <summary>
    /// Create a ONB from Z component of a Normal n using Duff et al. algorithm. The Normal n must be normalized.
    /// </summary>
    public void CreateOnbFromZ(Normal n)
    {
        var sign = MathF.CopySign(1.0f, n.Z);

        var a = -1.0f / (sign + n.Z);
        var b = n.X * n.Y * a;

        E1 = new Vec(1.0f + sign * n.X * n.X * a, sign * b, -sign * n.X);
        E2 = new Vec(b, sign + n.Y * n.Y * a, -n.Y);
        E3 = n.to_vec();
    }
}