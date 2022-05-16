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

    public Color eval(Normal normal, Vec inDir, Vec outDir, Vec2d uv)
    {
        return new Color(1, 1, 1);
    }

}

/// <summary>
/// A class representing an ideal diffuse BRDF (also called «Lambertian»)
/// </summary>
public class DiffuseBrdf: Brdf
{
    public DiffuseBrdf() : base(){}
    public DiffuseBrdf(IPigment pigment): base(pigment){}

    public Color eval(Normal normal, Vec inDir, Vec outDir, Vec2d uv)
    {
        return Pigment.get_color(uv) * (float)(1.0 / Math.PI);
    }
    
}