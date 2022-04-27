using Trace.Geometry;
using Trace.Cameras;


namespace Trace;

/// <summary>
/// Abstract class that represents a shape.
/// </summary>
abstract class Shape
{
    protected Transformation Transformation;
    
    protected Shape()
    {
        Transformation = new Transformation();
    }

    protected Shape(Transformation transformation)
    {
        Transformation = transformation;
    }

    public abstract HitRecord? ray_intersection(Ray ray);
}
