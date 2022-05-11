namespace Trace;

public class Material
{
   public IBrdf Brdf = new DiffuseBrdf();
   public Pigment EmittedRadiance = new UniformPigment(new Color(0, 0, 0));
}