using Trace.Geometry;

namespace Trace.Cameras;

/// <summary>
/// A Ray of light propagating in 3D space.
/// </summary>
public struct Ray
{
    /// <summary>
    /// The Origin Point of the Ray.
    /// </summary>
    public Point Origin { get; set; }
    
    /// <summary>
    /// The Vec direction of the Ray.
    /// </summary>
    public Vec Dir { get; set; }
    
    /// <summary>
    /// The minimum distance the Ray can travel (as Dir * tmin).
    /// </summary>
    public float Tmin { get; set; } = 1e-5f;
    
    /// <summary>
    /// The maximum distance the Ray can travel (as Dir * tmax).
    /// </summary>
    public float Tmax { get; set; } = float.PositiveInfinity;
    
    /// <summary>
    /// The number of times the Ray was reflected/refracted.
    /// </summary>
    public int Depth { get; set; } = 0;

    public Ray(Point origin, Vec dir, float tmin, float tmax, int depth)
    {
        Origin = origin;
        Dir = dir;
        Tmin = tmin;
        Tmax = tmax;
        Depth = depth;
    }

    public Ray(Point origin, Vec dir, float tmin = 1e-5f)
    {
        Origin = origin;
        Dir = dir;
        Tmin = tmin;
    }

    /// <summary>
    /// Returns the position of the Ray in 3D space at time t.
    /// </summary>
    public Point at(float t)
    {
        return Origin + Dir * t;
    }

    /// <summary>
    /// Checks if two Rays are equal within a epsilon threshold.
    /// </summary>
    public bool is_close(Ray ray, double epsilon = 1e-5)
    {
        return Origin.is_close(ray.Origin, epsilon) && Dir.is_close(ray.Dir, epsilon);
    }

    /// <summary>
    /// Applies a Transformation to the Ray.
    /// </summary>
    public Ray transform(Transformation transformation)
    {
        return new Ray(transformation * Origin, transformation * Dir, Tmin, Tmax, Depth);
    }
}