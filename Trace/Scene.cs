using Trace.Cameras;

namespace Trace;

/// <summary>
/// A Scene representing the image to render.
/// </summary>
public class Scene
{

    public World World = new();
    public ICamera? Camera = null;

    public Dictionary<string, Material> Materials = new();
    public Dictionary<string, float> FloatVariables = new();
    public List<string> OverriddenVariables = new();
    
}