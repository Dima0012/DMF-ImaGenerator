namespace Trace;

public struct Color
{
   private float r { get; set; } = 0.;
   private float g { get; set; } = 0.;
   private float b { get; set; } = 0.;

   public Color(float r, float g, float b)
   {
      this.r = r;
      this.g = g;
      this.b = b;
   }

}