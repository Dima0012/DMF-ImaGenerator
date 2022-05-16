namespace Trace;

public class Material
{
   public Brdf Brdf = new DiffuseBrdf();
   public IPigment EmittedRadiance = new UniformPigment(new Color(0, 0, 0));
}