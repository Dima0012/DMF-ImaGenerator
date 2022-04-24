using Trace.Geometry;


namespace Trace.Cameras;

/// <summary>
/// Interface representing an observer.
/// </summary>
public interface ICamera
{
    /// <summary>
    /// Fires a Ray through the camera, at screen coordinates (u,v).
    /// </summary>
    public Ray fire_ray(float u, float v);
}

/// <summary>
/// A class representing an observer seeing the world
///  through an Orthogonal projection 3D -> 2D.
/// </summary>
public class OrthogonalCamera : ICamera
{
    /// <summary>
    /// The Aspect Ration of the screen. Most common value is 16:9.
    /// </summary>
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


    /// <summary>
    /// Implementation of fire_ray for a Orthogonal Camera.
    /// </summary>
    public Ray fire_ray(float u, float v)
    {
        var origin = new Point(-1.0f, (1.0f - 2 * u) * AspectRatio, 2 * v - 1);
        var direction = new Vec(1.0f, 0.0f, 0.0f);
        return new Ray(origin, direction).transform(Transformation);
    }
}

/// <summary>
/// A class representing an observer seeing the world
///  through an Perspective projection 3D -> 2D.
/// </summary>
public class PerspectiveCamera : ICamera
{
    /// <summary>
    /// The distance of the observer from the screen.
    /// </summary>
    public float Distance { get; set; } = 1.0f;

    /// <summary>
    /// The Aspect Ration of the screen. Most common value is 16:9.
    /// </summary>
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


    /// <summary>
    /// Implementation of fire_ray for a Perspective Camera.
    /// </summary>
    public Ray fire_ray(float u, float v)
    {
        var origin = new Point(-Distance, 0.0f, 0.0f);
        var direction = new Vec(Distance, (1.0f - 2 * u) * AspectRatio, 2 * v - 1);
        return new Ray(origin, direction, 1.0f).transform(Transformation);
    }
}