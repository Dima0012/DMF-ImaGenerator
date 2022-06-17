using Trace.Cameras;

namespace Trace;

/// <summary>
///     A Scene representing the image to render.
/// </summary>
public class Scene
{
    public ICamera? Camera = null;
    public Dictionary<string, float> FloatVariables = new();

    public Dictionary<string, Material> Materials = new();
    public List<string> OverriddenVariables = new();

    public World World = new();
}