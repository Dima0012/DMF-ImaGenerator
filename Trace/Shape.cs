using Trace.Geometry;
using Trace.Cameras;


namespace Trace;

/// <summary>
/// A generic 3D shape. This is an abstract class and should only be used to derive concrete classes.
/// Be sure to redefine the method 'ray_intersection'.
/// </summary>
public abstract class Shape
{
    protected Transformation Transformation;
    
    public Shape()
    {
        Transformation = new Transformation();
    }

    public Shape(Transformation transformation)
    {
        Transformation = transformation;
    }

    public abstract HitRecord? ray_intersection(Ray ray);
}

public class Sphere : Shape
{
    public Sphere() : base(){}
    public Sphere(Transformation transformation) : base(transformation){}
    

    /// <summary>
    /// Checks if a ray intersects the sphere
    /// Return a `HitRecord`, or `None` if no intersection was found.
    /// </summary>
    public override HitRecord? ray_intersection(Ray ray)
    {
        var invRay = ray.transform(Transformation.inverse());
        var originVec = invRay.Origin.to_vec();
        var a = invRay.Dir.squared_norm();
        var b = 2.0 * (originVec * invRay.Dir);
        var c = originVec.squared_norm() - 1.0;

        var delta = b * b - 4.0 * a * c;
        if (delta <= 0)
        {
            return null;
        }

        var sqrtDelta = Math.Sqrt(delta);
        var tMin = (-b - sqrtDelta) / (2.0 * a);
        var tMax = (-b + sqrtDelta) / (2.0 * a);
        var firstHitT = 0.0f;

        if (tMin > invRay.Tmin && tMin < invRay.Tmax)
        {
            firstHitT = (float) tMin;
        }
        else if (tMax > invRay.Tmin && tMax < invRay.Tmax)
        {
            firstHitT = (float) tMax;
        }
        else
        {
            return null;
        }

        var hitPoint = invRay.at(firstHitT);
        return new HitRecord(
            Transformation * hitPoint, 
            Transformation * sphere_normal(hitPoint,invRay.Dir),
            sphere_point_to_uv(hitPoint),
            firstHitT, ray
            );

    }

    /// <summary>
    /// Compute the normal of a unit sphere
    /// The normal is computed for `point` (a point on the surface of the sphere),
    /// and it is chosen so that it is always in the opposite direction with respect to `ray_dir`.
    /// </summary>
    public Normal sphere_normal(Point point, Vec ray_dir)
    {
        var result = new Normal(point.X, point.Y, point.Z);
        if (point.to_vec() * ray_dir < 0.0)
        {
            return result;
        }
        return result.neg();
    }

    /// <summary>
    /// Convert a 3D point on the surface of the unit sphere into a (u, v) 2D point.
    /// </summary>
    public Vec2d sphere_point_to_uv(Point point)
    {
        float u = (float) (Math.Atan2(point.Y, point.X) / (2.0 * Math.PI));
        if (u < 0.0f)
        {
            u = u + 1.0f;
        }

        return new Vec2d(u, (float) (Math.Acos(point.Z) / Math.PI));
    }
}
