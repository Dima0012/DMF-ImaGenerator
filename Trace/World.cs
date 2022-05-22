using Trace.Cameras;
using Trace.Geometry;
//using Point = System.Drawing.Point; Why did we put this??

namespace Trace;

/// <summary>
/// A class holding a list of shape which make the world scene.
/// </summary>
public class World
{
    public List<Shape> Shapes { get; set; } = new();
    public List<PointLight> PointLights { get; set; } = new();

    /// <summary>
    /// Append a new shape to this world scene.
    /// </summary>
    public void add(Shape shape)
    {
        Shapes.Add(shape);
    }
    
    /// <summary>
    /// Append a new point light to this world.
    /// </summary>
    public void add_light(PointLight pointLight)
    {
        PointLights.Add(pointLight);
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

    /// <summary>
    /// Checks whether ray intersects any of the shapes or not.
    /// If present, returns the intersection closest to the Ray origin, otherwise returns null.
    /// </summary>
    public bool is_point_visible(Point point, Point observerPosition)
    {
        var direction = point - observerPosition;
        var dirNorm = direction.norm();

        var ray = new Ray(observerPosition, direction, 1e-2f / dirNorm, 1.0f, 0);
        foreach (var shape in Shapes)
        {
            if (shape.quick_ray_intersection(ray))
            {
                return false;
            }
        }

        return true;
    }
    
}