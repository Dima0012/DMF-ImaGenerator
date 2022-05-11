using Trace.Geometry;

namespace Trace;


public interface IBrdf
{
    public Pigment Pigment { get; set; }

    public Color eval(Normal normal, Vec inDir, Vec outDir, Vec2d uv);

}

public class DiffuseBrdf: IBrdf
{
    public Pigment Pigment { get; set; }

    public Color eval(Normal normal, Vec inDir, Vec outDir, Vec2d uv)
    {
        return Pigment.get_color(uv) * (1.0f / Math.PI); //dont know here what to put, reflectance is not present in pytracer
        // it should be as here, a constant
    }
    
}