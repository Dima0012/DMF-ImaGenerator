using Trace.Geometry;


namespace Trace.Cameras;

public interface ICamera
{
    public Ray fire_ray(float u, float v);
}

public class OrthogonalCamera : ICamera
{
    public float AspectRatio { get; set; } = 1.0f;
    public Transformation Transformation { get; set; }

    public OrthogonalCamera(float aspectRatio, Transformation transformation)
    {
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }

    public OrthogonalCamera(Transformation transformation)
    {
        Transformation = transformation;
    }


    public Ray fire_ray(float u, float v)
    {
        var origin = new Point(-1.0f, (1.0f - 2 * u) * AspectRatio, 2 * v - 1);
        var direction = new Vec(1.0f, 0.0f, 0.0f);
        return new Ray(origin, direction).transform(Transformation);
    }
}

public class PerspectiveCamera : ICamera
{
    public float Distance { get; set; } = 1.0f;
    public float AspectRatio { get; set; } = 1.0f;
    public Transformation Transformation { get; set; }

    public PerspectiveCamera(float distance, float aspectRatio, Transformation transformation)
    {
        Distance = distance;
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }
    
    public PerspectiveCamera(float aspectRatio, Transformation transformation)
    {
        AspectRatio = aspectRatio;
        Transformation = transformation;
    }

    public PerspectiveCamera(Transformation transformation)
    {
        Transformation = transformation;
    }


    public Ray fire_ray(float u, float v)
    {
        var origin = new Point(-Distance, 0.0f, 0.0f);
        var direction = new Vec(Distance, (1.0f - 2 * u) * AspectRatio, 2 * v - 1);
        return new Ray(origin, direction, 1.0f).transform(Transformation);
    }
}