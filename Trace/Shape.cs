using Trace.Cameras;
using Trace.Geometry;

namespace Trace;

/// <summary>
///     A generic 3D shape. This is an abstract class and should only be used to derive concrete classes.
///     Be sure to redefine the method 'ray_intersection'.
/// </summary>
public abstract class Shape
{
    public Material Material;
    public Transformation Transformation;

    public Shape()
    {
        Transformation = new Transformation();
        Material = new Material();
    }

    public Shape(Transformation transformation)
    {
        Transformation = transformation;
        Material = new Material();
    }

    public Shape(Transformation transformation, Material material)
    {
        Transformation = transformation;
        Material = material;
    }

    /// <summary>
    ///     Checks if a ray intersects the shape.
    ///     Returns a HitRecord, or Null if no intersection was found.
    /// </summary>
    public abstract HitRecord? ray_intersection(Ray ray);

    /// <summary>
    ///     Determine whether a ray hits the shape or not.
    /// </summary>
    public abstract bool quick_ray_intersection(Ray ray);
}

public class Sphere : Shape
{
    public Sphere()
    {
    }

    public Sphere(Transformation transformation) : base(transformation)
    {
    }

    public Sphere(Transformation transformation, Material material) : base(transformation, material)
    {
    }


    /// <summary>
    ///     Checks if a ray intersects the sphere.
    ///     Return a HitRecord, or Null if no intersection was found.
    /// </summary>
    public override HitRecord? ray_intersection(Ray ray)
    {
        var invRay = ray.transform(Transformation.inverse());
        var originVec = invRay.Origin.to_vec();
        var a = invRay.Dir.squared_norm();
        var b = 2.0 * (originVec * invRay.Dir);
        var c = originVec.squared_norm() - 1.0;

        var delta = b * b - 4.0 * a * c;
        if (delta <= 0) return null;

        var sqrtDelta = Math.Sqrt(delta);
        var tMin = (-b - sqrtDelta) / (2.0 * a);
        var tMax = (-b + sqrtDelta) / (2.0 * a);
        var firstHitT = 0.0f;

        if (tMin > invRay.Tmin && tMin < invRay.Tmax)
            firstHitT = (float) tMin;
        else if (tMax > invRay.Tmin && tMax < invRay.Tmax)
            firstHitT = (float) tMax;
        else
            return null;

        var hitPoint = invRay.at(firstHitT);
        return new HitRecord(
            Transformation * hitPoint,
            Transformation * sphere_normal(hitPoint, invRay.Dir),
            sphere_point_to_uv(hitPoint),
            firstHitT,
            ray,
            this
        );
    }

    /// <summary>
    ///     Compute the normal of a unit sphere.
    ///     The normal is computed for point (a point on the surface of the sphere),
    ///     and it is chosen so that it is always in the opposite direction with respect to ray_dir.
    /// </summary>
    public Normal sphere_normal(Point point, Vec ray_dir)
    {
        var result = new Normal(point.X, point.Y, point.Z);
        if (point.to_vec() * ray_dir < 0.0) return result;
        return result.neg();
    }

    /// <summary>
    ///     Convert a 3D point on the surface of the unit sphere into a (u, v) 2D point.
    /// </summary>
    public Vec2d sphere_point_to_uv(Point point)
    {
        var u = (float) (Math.Atan2(point.Y, point.X) / (2.0 * Math.PI));
        if (u < 0.0f) u += 1.0f;

        return new Vec2d(u, (float) (Math.Acos(point.Z) / Math.PI));
    }

    /// <summary>
    ///     Quickly checks if a ray intersects the sphere.
    /// </summary>
    public override bool quick_ray_intersection(Ray ray)
    {
        var invRay = ray.transform(Transformation.inverse());
        var originVec = invRay.Origin.to_vec();
        var a = invRay.Dir.squared_norm();
        var b = 2.0f * originVec * invRay.Dir;
        var c = originVec.squared_norm() - 1.0f;

        var delta = b * b - 4.0f * a * c;
        if (delta <= 0.0) return false;

        var sqrtDelta = MathF.Sqrt(delta);
        var tmin = (-b - sqrtDelta) / (2.0f * a);
        var tmax = (-b + sqrtDelta) / (2.0f * a);

        return (invRay.Tmin < tmin && tmin < invRay.Tmax) || (invRay.Tmin < tmax && tmax < invRay.Tmax);
    }
}

/// <summary>
///     A 3D infinite plane parallel to the x and y axis and passing through the origin.
/// </summary>
public class Plane : Shape
{
    public Plane()
    {
    }

    public Plane(Material material)
    {
        Material = material;
    }

    public Plane(Transformation transformation) : base(transformation)
    {
    }

    public Plane(Transformation transformation, Material material) : base(transformation, material)
    {
    }

    /// <summary>
    ///     Checks if a ray intersects the plane.
    ///     Return a HitRecord, or Null if no intersection was found.
    /// </summary>
    public override HitRecord? ray_intersection(Ray ray)
    {
        var invRay = ray.transform(Transformation.inverse());

        if (Math.Abs(invRay.Dir.Z) < 1e-5) return null;

        var t = -invRay.Origin.Z / invRay.Dir.Z;

        if (t <= invRay.Tmin || t >= invRay.Tmax) return null;

        var hitPoint = invRay.at(t);
        var planeNormal = new Normal(0.0f, 0.0f, 1.0f);
        if (invRay.Dir.Z >= 0) planeNormal.Z = -1.0f;


        return new HitRecord(
            Transformation * hitPoint,
            Transformation * planeNormal,
            new Vec2d(hitPoint.X - (float) Math.Floor(hitPoint.X), hitPoint.Y - (float) Math.Floor(hitPoint.Y)),
            t,
            ray,
            this
        );
    }

    /// <summary>
    ///     Quickly checks if a ray intersects the plane.
    /// </summary>
    public override bool quick_ray_intersection(Ray ray)
    {
        var invRay = ray.transform(Transformation.inverse());
        if (Math.Abs(invRay.Dir.Z) < 1e-5) return false;

        var t = -invRay.Origin.Z / invRay.Dir.Z;
        return invRay.Tmin < t && t < invRay.Tmax;
    }
}