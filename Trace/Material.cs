namespace Trace;

public class Material
{
    public Material()
    {
        Brdf = new DiffuseBrdf();
        EmittedRadiance = new UniformPigment(new Color(0, 0, 0));
    }

    public Material(Brdf brdf)
    {
        Brdf = brdf;
        EmittedRadiance = new UniformPigment(new Color(0, 0, 0));
    }

    public Material(Brdf brdf, IPigment emittedRadiance)
    {
        Brdf = brdf;
        EmittedRadiance = emittedRadiance;
    }

    public Brdf Brdf { get; set; }
    public IPigment EmittedRadiance { get; set; }
}