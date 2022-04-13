using Trace.Geometry;

namespace Trace.Cameras;

public struct Ray
{
    public Point Origin { get; set; }
    public Vec Dir { get; set; }
    public float Tmin { get; set; } = 1e-5f;
    public float Tmax { get; set; } = float.PositiveInfinity;
    public int Depth { get; set; } = 0;

    public Ray(Point origin, Vec dir, float tmin, float tmax, int depth)
    {
        Origin = origin;
        Dir = dir;
        Tmin = tmin;
        Tmax = tmax;
        Depth = depth;
    }

    public Ray(Point origin, Vec dir)
    {
        Origin = origin;
        Dir = dir;
    }

    public Point at(float t)
    {
        return Origin + Dir * t;
    }

    public bool is_close(Ray ray, double epsilon = 1e-5)
    {
        return Origin.is_close(ray.Origin, epsilon) && Dir.is_close(ray.Dir, epsilon);
    }

    public Ray transform(Transformation transformation)
    {
        return new Ray(transformation * Origin, transformation * Dir, Tmin, Tmax, Depth);
    }
}