using Trace.Cameras;

namespace Trace;

/// <summary>
/// An interface representing a Renderer.
/// </summary>
public abstract class Renderer
{
    /// <summary>
    /// The world scene.
    /// </summary>
    protected World World;
    
    /// <summary>
    /// The background color of the scene. Default is black. 
    /// </summary>
    protected Color BackgroundColor;
    
    public Renderer(World world)
    {
        World = world;
        BackgroundColor = new Color(0, 0, 0);
    }
    
    public Renderer(World world, Color backgroundColor)
    {
        World = world;
        BackgroundColor = backgroundColor;
    }

    /// <summary>
    /// Estimates the radiance along a ray with a specific method.
    /// </summary>
    public virtual Color Render(Ray ray)
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// This Renderer will return just one color. We need it for ImageTracerTests.
/// </summary>
public class OneColor : Renderer
{
    public OneColor(World world) : base(world){}
    public override Color Render(Ray ray)
    {
        return new Color(1f, 2f, 3f);
    }
}

/// <summary>
/// This Renderer implements a test render for TestAntiAliasing.
/// For debug purposes only.
/// </summary>
public class AliasingRenderer : Renderer
{
    public int NumOfRays { get; set; }
    public int Test1 { get; set; }
    public int Test2 { get; set; }
    public int Test3 { get; set; }
    
    public AliasingRenderer(World world) : base(world)
    {
        Test1 = 0;
        Test2 = 0;
        Test3 = 0;
        NumOfRays = 0;
    }

    public override Color Render(Ray ray)
    {
        var point = ray.at(1);
    
        // Check all the rays intersect the screen within the region [−1, 1] × [−1, 1]
        var test1 = MathF.Abs(point.X) < 10e-5;
        var test2 = point.Y is >= -1.0f and <= 1.0f;
        var test3 = point.Z is >= -1.0f and <= 1.0f;

        if (test1 && test2 && test3)
        {
            Test1 += 1;
            Test2 += 1;
            Test3 += 1;
        }
        
        NumOfRays += 1;
        
        return new Color(0, 0, 0);

    }
    
}

/// <summary>
/// This Renderer implements an on-off method. All objects will be rendered white, the background black.
/// For debug purposes only.
/// </summary>
public class OnOffRenderer : Renderer
{
    public Color ObjectColor = new(1, 1, 1);

    public OnOffRenderer(World world) : base(world){}
    public OnOffRenderer(World world, Color backgroundColor) : base(world, backgroundColor){}
    
    public override Color Render(Ray ray)
    {
        return World.ray_intersection(ray) != null ? ObjectColor : BackgroundColor;
    }
}


/// <summary>
/// A "flat" renderer. <br />
/// Estimates the solution of the rendering equation by neglecting any contribution of the light.
/// It uses the pigment of each surface to determine how to compute the final radiance.
/// </summary>
public class FlatRenderer : Renderer
{
    
    public FlatRenderer(World world) : base(world){}
    public FlatRenderer(World world, Color backgroundColor) : base(world, backgroundColor){}
    
    public override Color Render(Ray ray)
    {
        var hit = World.ray_intersection(ray);

        if (hit is null)
        {
            return BackgroundColor;
        }

        var material = hit.Shape.Material;

        return material.Brdf.Pigment.get_color(hit.SurfacePoint) + material.EmittedRadiance.get_color(hit.SurfacePoint);

    }
}

/// <summary>
/// A simple path-tracing renderer. The algorithm implemented here allows the caller to tune number of
/// rays thrown at each iteration, as well as the maximum depth. It implements Russian roulette, so
/// in principle it will take a finite time to complete the calculation even if you set max_depth
/// to Math.Inf.
/// </summary>
public class PathTracer : Renderer
{
    public Pcg Pcg { get; set; }
    public int NumOfRays{ get; set; }
    public int MaxDepth{ get; set; }
    public int RussianRouletteLimit{ get; set; }
    public PathTracer(World world) : base(world)
    {
        Pcg = new Pcg();
        NumOfRays = 10;
        MaxDepth = 2;
        RussianRouletteLimit = 3;
    }
    public PathTracer(World world, Color backgroundColor) : base(world, backgroundColor)
    {
        Pcg = new Pcg();
        NumOfRays = 10;
        MaxDepth = 2;
        RussianRouletteLimit = 3;
    }
    
    public PathTracer(World world, Pcg pcg, int numOfRays, int maxDepth, int russianRouletteLimit) : base(world)
    {
        Pcg = pcg;
        NumOfRays = numOfRays;
        MaxDepth = maxDepth;
        RussianRouletteLimit = russianRouletteLimit;
    }

    public override Color Render(Ray ray)
    {
        if (ray.Depth > MaxDepth)
        {
            return new Color(0.0f, 0.0f, 0.0f);
        }
        
        var hitRecord = World.ray_intersection(ray);
        if (hitRecord == null)
        {
            return BackgroundColor;
        }

        var hitMaterial = hitRecord.Shape.Material;
        var hitColor = hitMaterial.Brdf.Pigment.get_color(hitRecord.SurfacePoint);
        var emittedRadiance = hitMaterial.EmittedRadiance.get_color(hitRecord.SurfacePoint);

        var hitColorLum = Math.Max(Math.Max(hitColor.R, hitColor.G), hitColor.B);

        //Russian roulette
        if (ray.Depth >= RussianRouletteLimit)
        {
            var q = MathF.Max(0.05f, 1 - hitColorLum);
            if (Pcg.random_float() > q)
            {
                //Keep the recursion going, but compensate for other potentially discarded rays
                hitColor *= 1.0f / (1.0f - q);
            }
            else
            {
                //Terminate prematurely
                return emittedRadiance;
            }

        }

        var cumRadiance = new Color(0.0f, 0.0f, 0.0f);
        if (hitColorLum > 0.0) //Only do costly recursions if it's worth it
        {
            for (int rayIndex = 0; rayIndex < NumOfRays; rayIndex++)
            {
                var newRay = hitMaterial.Brdf.scatter_ray(
                    Pcg,
                    hitRecord.Ray.Dir,
                    hitRecord.WorldPoint,
                    hitRecord.Normal,
                    ray.Depth + 1
                );
                // Recursive call
                var newRadiance = Render(newRay);
                cumRadiance += hitColor * newRadiance;
            }
        }

        return emittedRadiance + cumRadiance * (1.0f / NumOfRays);
        
    }

}

/// <summary>
/// A simple point-light renderer.
/// This renderer is similar to what POV-Ray provides by default.
/// </summary>
public class PointLightRenderer : Renderer
{
    public Color AmbientColor;

    public PointLightRenderer(World world) : base(world)
    {
        AmbientColor = new Color(0.1f, 0.1f, 0.1f);
    }
    
    public PointLightRenderer(World world, Color backgroundColor) : base(world, backgroundColor)
    {
        AmbientColor = new Color(0.1f, 0.1f, 0.1f);
    }
    
    public PointLightRenderer(World world, Color backgroundColor, Color ambientColor) : base(world, backgroundColor)
    {
        AmbientColor = ambientColor;
    }

    public override Color Render(Ray ray)
    {
        var hitRecord = World.ray_intersection(ray);
        if (hitRecord == null)
        {
            return BackgroundColor;
        }

        var hitMaterial = hitRecord.Shape.Material;

        var resultColor = AmbientColor;
        foreach (var curLight in World.PointLights)
        {
            if (!World.is_point_visible(curLight.Position, hitRecord.WorldPoint)) continue;
            var distanceVec = hitRecord.WorldPoint - curLight.Position;
            var distance = distanceVec.norm();
            var inDir = distanceVec * (1.0f / distance);
            var cosTheta = MathF.Max(0.0f, hitRecord.Normal.normalized_dot(ray.Dir.neg()));

            var distanceFactor = 1.0f;
            if (curLight.LinearRadius > 0)
            {
                distanceFactor = MathF.Pow(curLight.LinearRadius / distance, 2);
            }

            var emittedColor = hitMaterial.EmittedRadiance.get_color(hitRecord.SurfacePoint);
            var brdfColor = hitMaterial.Brdf.eval(
                hitRecord.Normal,
                inDir,
                ray.Dir.neg(),
                hitRecord.SurfacePoint);
            resultColor += (emittedColor + brdfColor) * curLight.Color * cosTheta * distanceFactor;
        }

        return resultColor;
    }
}
    

