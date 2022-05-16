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
    protected Color BackgroundColor = new(0, 0, 0);
    
    protected Renderer(World world)
    {
        World = world;
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
/// This Renderer implements an on-off method. All objects will be rendered white, the background black.
/// For debug purposes only.
/// </summary>
public class OnOffRenderer : Renderer
{
    public Color ObjectColor = new(1, 1, 1);

    public OnOffRenderer(World world) : base(world){}
    
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