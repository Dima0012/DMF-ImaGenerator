using Trace.Geometry;
using Trace.Cameras;


namespace Trace;

/// <summary>
/// A class holding information about a ray-shape intersection
/// </summary>
public class HitRecord
{
    public Point WorldPoint { get; set; }
    public Normal Normal { get; set; }
    public Vec2d SurfacePoint { get; set; }
    public float T { get; set; }
    public Ray Ray { get; set; }
    public Shape Shape { get; set; }

    public HitRecord(Point worldPoint, Normal normal, Vec2d surfacePoint, float t, Ray ray, Shape shape)
    {
        WorldPoint = worldPoint;
        Normal = normal;
        SurfacePoint = surfacePoint;
        T = t;
        Ray = ray;
        Shape = shape;
    }

    /// <summary>
    ///Check whether two `HitRecord` represent the same hit event or not
    /// </summary>
    public bool is_close(HitRecord? hr, double epsilon = 1e-5)
    {
        return WorldPoint.is_close(hr!.WorldPoint) &&
               Normal.is_close(hr.Normal) &&
               SurfacePoint.is_close(hr.SurfacePoint) &&
               (Math.Abs(T - hr.T) < epsilon) &&
               Ray.is_close(hr.Ray);
    }
}