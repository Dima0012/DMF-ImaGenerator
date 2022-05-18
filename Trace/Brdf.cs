using Trace.Cameras;
using Trace.Geometry;

namespace Trace;

/// <summary>
/// An abstract class representing a Bidirectional Reflectance Distribution Function
/// </summary>
public abstract class Brdf
{
    public IPigment Pigment;

    public Brdf()
    {
        Pigment = new UniformPigment(new Color(1, 1, 1));
    }
    
    public Brdf(IPigment pigment)
    {
        Pigment = pigment;
    }

    public virtual Color eval(Normal normal, Vec inDir, Vec outDir, Vec2d uv)
    {
        return new Color(1, 1, 1);
    }
    
    public abstract Ray scatter_ray(Pcg pcg, Vec inDir, Point intP, Normal n, int depth);

}

/// <summary>
/// A class representing an ideal diffuse BRDF (also called «Lambertian»)
/// </summary>
public class DiffuseBrdf: Brdf
{
    public DiffuseBrdf() : base(){}
    public DiffuseBrdf(IPigment pigment): base(pigment){}

    public override Color eval(Normal normal, Vec inDir, Vec outDir, Vec2d uv)
    {
        return Pigment.get_color(uv) * (float)(1.0 / Math.PI);
    }
    
    public override Ray scatter_ray(Pcg pcg, Vec inDir, Point intP, Normal n, int depth)
    {
        //Cosine-weighted distribution around the z (local) axis
        var onb = new Onb(n.to_vec());
        var cosThetaSq = pcg.random_float();
        var cosTheta = MathF.Sqrt(cosThetaSq);
        var sinTheta = MathF.Sqrt(1.0f - cosThetaSq);
        var phi = 2.0f * MathF.PI * pcg.random_float();

        return new Ray(
            intP,
            onb.E1 * MathF.Cos(phi) * cosTheta + onb.E2 * MathF.Sin(phi) * cosTheta + onb.E3 * sinTheta,
            1e-3f,
            float.PositiveInfinity,
            depth);
    }
    
}

/// <summary>
/// A class representing an ideal mirror BRDF
/// </summary>
public class SpecularBrdf: Brdf
{
    public float ThresholdAngleRad;
    public SpecularBrdf() : base(){}

    public SpecularBrdf(IPigment pigment, float thresholdAngleRad) : base(pigment)
    {
        ThresholdAngleRad = thresholdAngleRad;
    }
    
    /// <summary>
    /// We provide this implementation for reference, but we are not going to use it (neither in the
    /// path tracer nor in the point-light tracer)
    /// </summary>
    public override Color eval(Normal normal, Vec inDir, Vec outDir, Vec2d uv)
    {
        var thetaIn = Math.Acos(normal.normalized_dot(inDir));
        var thetaOut = Math.Acos(normal.normalized_dot(outDir));

        if (Math.Abs(thetaIn - thetaOut) < ThresholdAngleRad)
        {
            return Pigment.get_color(uv);
        }

        return new Color(0.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// There is no need to use the PCG here, as the reflected direction is always completely
    /// deterministic for a perfect mirror
    /// </summary>
    public override Ray scatter_ray(Pcg pcg, Vec inDir, Point intP, Normal n, int depth)
    {
        var rayDir = new Vec(inDir.X, inDir.Y, inDir.Z).normalize();
        var normal = n.to_vec();
        var dotProd = normal * rayDir;

        return new Ray(
            intP,
            rayDir - normal * (float) (2 * dotProd),
            1e-5f,
            float.PositiveInfinity,
            depth);
    }
    
}