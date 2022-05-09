using Trace.Cameras;

namespace Trace;

/// <summary>
/// A class holding a list of shape which make the world scene.
/// </summary>
public class World
{
    public List<Shape> Shapes { get; set; } = new();

    /// <summary>
    /// Append a new shape to this world scene.
    /// </summary>
    public void add(Shape shape)
    {
        Shapes.Add(shape);
    }

    /// <summary>
    /// Checks whether ray intersects any of the shapes or not.
    /// If present, returns the intersection closest to the Ray origin, otherwise returns null.
    /// </summary>
    public HitRecord? ray_intersection(Ray ray)
    {
        HitRecord? closest = null;

        foreach (var shape in Shapes)
        {
            var intersection = shape.ray_intersection(ray);
            if (intersection is null)
            {
                continue;
            }

            if (closest is null || intersection.T < closest.T)
            {
                closest = intersection;
            }
        }

        return closest;
    }
}